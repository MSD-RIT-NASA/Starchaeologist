import PySimpleGUI as sg
import matplotlib.pyplot as plt
import os
from matplotlib.animation import FuncAnimation
import base64
import serial.tools.list_ports
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import collections
import sys
import time
from threading import Thread, Event
import subprocess
import server

BUTTON_SIZE = (24,1)
CALIBRATION_COOLDOWN = 5

root_path = os.path.dirname(__file__)
image_path = root_path+"\\images"
com_ports = []
start_server = Event()
stop_server = Event()
calibrate_sensors = Event()
stop_calibration = Event()
reset_actuators = Event()
stop_actuators = Event()

def draw_figure(canvas, figure):
    figure_canvas_agg = FigureCanvasTkAgg(figure, canvas)
    figure_canvas_agg.draw()
    figure_canvas_agg.get_tk_widget().pack(side='top', fill='both', expand=1)
    return figure_canvas_agg

def updateComPorts():
    global com_ports
    ports = serial.tools.list_ports.comports()
    com_ports = []
    for port, desc, hwid in sorted(ports):
        if desc.__contains__('USB'):
            com_ports.append(port)
updateComPorts()

base_image = open(image_path + '\\base_img.png', 'rb').read()
base_image = base64.b64encode(base_image)

planet_img = open(image_path + '\\planet_img.png', 'rb').read()
planet_img = base64.b64encode(planet_img)

level_select = [[sg.Button('BASE Levels', font=("Arial", 24, "bold"), image_data=base_image, 
            button_color=('white',sg.theme_background_color()),border_width=0, key='base_layout'),
            sg.Button('PLANET Levels', font=("Arial", 24, "bold"), image_data=planet_img, 
            button_color=('white',sg.theme_background_color()),border_width=0, key='planet_layout')]]

base_col1 = sg.Column([
    [sg.Frame('Control:',[[sg.Button('Start Server', key='-start_server-', size=BUTTON_SIZE, button_color=('white', 'red'))],[sg.Button('Stop Server', key='-stop_server-', size=BUTTON_SIZE, button_color=('white', 'black'), disabled=True)], \
                                [sg.Button('Calibrate Floor', key='-calibrate_floor-', size=BUTTON_SIZE, button_color=('white', 'green'), disabled=True)], [sg.Button('Reset Actuators', key='-reset_actuators-', size=BUTTON_SIZE, button_color=('white', 'blue'), disabled=True)], \
                                [sg.Button('Stop Actuators', key='-stop_actuators-', size=BUTTON_SIZE, button_color=('black', 'yellow'), disabled=True)]], element_justification='center', expand_x=True, expand_y=True),
    sg.Frame('Configuration:',[[sg.Text('COM Port Number:'), sg.DropDown(com_ports, key='-com_port-', auto_size_text=True, readonly=True, size=(7,1))], \
                               [sg.Button('Update List', key='-update_com_ports-', size=BUTTON_SIZE, button_color=('white', 'black'))], \
                                [sg.Checkbox('Verbose Output', key='-verbose-', default=False, expand_x=True)]], expand_y=True, element_justification='center')],
    [sg.Frame('Difficulty:', [[sg.Slider(range=(0, 1), resolution=0.01, default_value=0.5, expand_x=True, enable_events=True, orientation='horizontal', key='-difficulty-'), sg.Button('Apply', key='-applyDifficulty-', size=(12,1), button_color=('white', 'blue'))]], expand_x=True, expand_y=True, element_justification='center')],
    [sg.Frame('Console Output:', [[sg.Button('Save Output to File', key='-saveOutput-', size=BUTTON_SIZE, button_color=('white', 'blue')), sg.Button('Clear Output', key='-clearOutput-', size=BUTTON_SIZE, button_color=('white', 'black'))], [sg.Output(background_color='black', text_color='white', size=(60,30), font=("Courier New", 8), key="-consoleOutput-")]], expand_x=True, expand_y=True, element_justification='center')]],expand_x=True, expand_y=True)
    #])

base_visualization = sg.Canvas(key='figCanvas')

base_col2 = sg.Column([
    [sg.Frame('Data Visualization:',[[sg.Button('Save Plots to File', key='-savePlot-', size=BUTTON_SIZE, button_color=('white', 'blue')), sg.Button('Clear Plots', key='-clearPlot-', size=BUTTON_SIZE, button_color=('white', 'black'))], [base_visualization]], expand_x=True, expand_y=True)]],expand_x=True, expand_y=True)

base_panel = [[base_col1, base_col2]]
planet_panel = [[]]

layout = [[sg.Button('Level Select', key='level_select'), sg.Push(), sg.Button('Quit', key='Exit')], \
          [sg.Push(), sg.Column(level_select, key='-level_select-'), sg.Column(base_panel, visible=False, key='-base_layout-'), sg.Column(planet_panel, visible=False, key='-planet_layout-'), sg.Push()]]

window = sg.Window("Starchaeologist Control Panel", layout, auto_size_buttons=True, default_button_element_size=(12,1), \
                   use_default_focus=False, finalize=True, resizable=True)
window.maximize()

#initial GUI layout, do not change
layout = 'level_select'
comm_port = ""
initial_difficulty = 0.5
calibrate_timer = 0
calibrate_start = 0
#GUI loop

#################################################################################################
#Animated plot test code
def my_function(i):
    cpu.append(0)
    #ax.plot(cpu, c='#EC5E29')
    #ax.scatter(len(cpu)-1, cpu[-1], c='#EC5E29')
    axes_1 = plt.subplot(2, 1, 1)
    axes_1.scatter(len(cpu)-1, cpu[-1], c='#EC5E29')
    axes_2 = plt.subplot(2, 2, 3)
    axes_2.scatter(len(cpu)-1, cpu[-1], c='#EC5E29')
    axes_3 = plt.subplot(2, 2, 4)
    axes_3.scatter(len(cpu)-1, cpu[-1], c='#EC5E29')
    fig.tight_layout()

cpu = collections.deque()
fig = plt.figure(figsize=(12,6), facecolor='#64778d')
fig.tight_layout()
animation = FuncAnimation(plt.gcf(), my_function, interval=1000, cache_frame_data=False)
animated_plot=draw_figure(base_visualization.TKCanvas, fig)
#################################################################################################

def calibrateDisableButton(calibrate_start_time, calibrate_timer, stop_server):
    while calibrate_timer == 1:
        if stop_server.is_set():
            break
        current_time = time.time()
        if current_time - calibrate_start_time > CALIBRATION_COOLDOWN:
            window['-calibrate_floor-'].update(disabled=False)
            calibrate_timer = 0

while True:
    event, values = window.read()

    ###############################################################
    # LEVEL SELECT
    ###############################################################
    if event in (None, 'Exit'):
        break
    if event in ('level_select', 'base_layout', 'planet_layout'):
        window[f'-{layout}-'].update(visible=False)
        layout = event
        window[f'-{event}-'].update(visible=True)

    ###############################################################
    # BASE PANEL
    ###############################################################
    # Start server thread
    if event == '-start_server-':
        server_thread = Thread(target=server.run, args=(start_server, stop_server, calibrate_sensors, reset_actuators, stop_actuators))
        server_thread.start()
        start_server.set()
        window['-start_server-'].update(disabled=True)
        window['-stop_server-'].update(disabled=False)
        window['-calibrate_floor-'].update(disabled=False)
        window['-reset_actuators-'].update(disabled=False)
        window['-stop_actuators-'].update(disabled=False)
    # Kill server thread
    if event == '-stop_server-':
        stop_server.set()
        window['-stop_server-'].update(disabled=True)
        window['-start_server-'].update(disabled=False)
        window['-calibrate_floor-'].update(disabled=True)
        window['-reset_actuators-'].update(disabled=True)
        window['-stop_actuators-'].update(disabled=True)
    if event == '-calibrate_floor-':
        #window['-calibrate_floor-'].update(disabled=True)
        calibrate_sensors.set()
        #calibrate_start_time = time.time()
        #calibrate_timer = 1
        #calibratebuttonDisable = Thread(target=calibrateDisableButton, args=(calibrate_start_time, calibrate_timer, stop_server))
        #calibratebuttonDisable.start()
    if event == '-com_port-':
        com_port = values['-com_port-']
        print(com_port)
    if event in ('-update_com_ports-'):
        updateComPorts()
        window['-com_port-'].update(values=com_ports)
    if event == '-difficulty-':
        initial_difficulty=values['-difficulty-']
    if event == '-applyDifficulty-':
        #TODO: send int(values['-difficulty-']) to server
        print("Difficulty set to", initial_difficulty)
    if event == '-clearOutput-':
        window.FindElement('-consoleOutput-').Update('')
    if event == '-clearPlot-':
        cpu.clear()
        plt.clf()
        animated_plot.get_tk_widget().forget()
        plt.subplot(2, 1, 1)
        plt.subplot(2, 2, 3)
        plt.subplot(2, 2, 4)
        fig.tight_layout()
        animated_plot=draw_figure(base_visualization.TKCanvas, fig)
stop_server.set()
window.close()
