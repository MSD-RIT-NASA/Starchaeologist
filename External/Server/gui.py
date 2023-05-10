import os, time, base64, subprocess
import PySimpleGUI as sg
import matplotlib.pyplot as plt
from matplotlib.animation import FuncAnimation
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
import serial.tools.list_ports
from threading import Thread, Event
from queue import Queue
import server
import collections

BUTTON_SIZE = (24,1)
CALIBRATION_COOLDOWN = 5

root_path = os.path.dirname(__file__)
image_path = root_path+"\\images"
com_ports = []
taskQueue = Queue()
responseQueue = Queue()

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
        if desc.__contains__('Arduino'):
            com_ports.append(port)
updateComPorts()

def applyComPort(com_port):
    if com_port not in ("", None):
        com_port = values['-com_port-']
        taskQueue.put(['updateCOM', com_port])

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
                               [sg.Button('Apply', key='-apply_com_port-', size=BUTTON_SIZE, button_color=('white', 'green'))], \
                               [sg.Button('Update List', key='-update_com_ports-', size=BUTTON_SIZE, button_color=('white', 'black'))], \
                                [sg.Checkbox('Verbose Output', key='-verbose-', default=False, expand_x=True)]], expand_y=True, element_justification='center')],
    [sg.Frame('Difficulty:', [[sg.Slider(range=(0, 1), resolution=0.01, default_value=0.5, expand_x=True, enable_events=True, orientation='horizontal', key='-difficulty-'), sg.Button('Apply', key='-applyDifficulty-', size=(12,1), button_color=('white', 'blue'))]], expand_x=True, expand_y=True, element_justification='center')],
    [sg.Frame('Console Output:', [[sg.Button('Save Output to File', key='-saveOutput-', size=BUTTON_SIZE, button_color=('white', 'blue')), sg.Button('Clear Output', key='-clearOutput-', size=BUTTON_SIZE, button_color=('white', 'black'))]], expand_x=True, expand_y=True, element_justification='center')]],expand_x=True, expand_y=True)
    #])

base_visualization = sg.Canvas(key='figCanvas')

planet_col1 = sg.Column([
    [sg.Frame('Control:',[[sg.Button('Start Server', key='-start_planet_server-', size=BUTTON_SIZE, button_color=('white', 'red'))], \
                          [sg.Button('Stop Server', key='-stop_planet_server-', size=BUTTON_SIZE, button_color=('white', 'black'), disabled=True)], \
                          [sg.Button('Launch Game', key='-launch_planet_game-', size=BUTTON_SIZE, button_color=('white', 'purple'), disabled=True)], \
                          [sg.Button('Quit Game', key='-quit_planet_game-', size=BUTTON_SIZE, button_color=('white', 'magenta'), disabled=True)]], \
                            element_justification='center', expand_x=True, expand_y=True)]])

base_panel = [[base_col1]]
planet_panel = [[planet_col1]]

layout = [[sg.Button('Level Select', key='level_select'), sg.Push(), sg.Button('Quit', key='Exit')], \
          [sg.Push(), sg.Column(level_select, key='-level_select-'), sg.Column(base_panel, visible=False, key='-base_layout-'), sg.Column(planet_panel, visible=False, key='-planet_layout-'), sg.Push()]]

window = sg.Window("Starchaeologist Control Panel", layout, auto_size_buttons=True, default_button_element_size=(12,1), \
                   use_default_focus=False, finalize=True, resizable=True)
#window.maximize()

#initial GUI layout, do not change
current_layout = 'level_select'
comm_port = ""
gameDifficulty = 0.5
calibrate_timer = 0
calibrate_start = 0
#GUI loop

#################################################################################################
#Animated plot test code
def my_function(i):
    cpu.append(0)
   #axes_1.plot(cpu, c='#EC5E29')
    #axes_1.scatter(len(cpu)-1, cpu[-1], c='#EC5E29')
    axes_1 = plt.subplot(2, 1, 1)
    axes_1.scatter(len(cpu)-1, cpu[-1], c='#EC5E29', marker='none')
    axes_2 = plt.subplot(2, 2, 3)
    axes_2.scatter(len(cpu)-1, cpu[-1], c='#EC5E29', marker='none')
    axes_3 = plt.subplot(2, 2, 4)
    axes_3.scatter(len(cpu)-1, cpu[-1], c='#EC5E29', marker='none')
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
        window[f'-{current_layout}-'].update(visible=False)
        current_layout = event
        window[f'-{event}-'].update(visible=True)

    ###############################################################
    # BASE PANEL
    ###############################################################
    # Start server thread
    if event == '-start_planet_server-':
        taskQueue = Queue()
        responseQueue = Queue()
        server_thread = Thread(target=server.run, args=(taskQueue, responseQueue))
        server_thread.start()
        taskQueue.put(['startServer'])
        window['-start_planet_server-'].update(disabled=True)
        window['-stop_planet_server-'].update(disabled=False)
        window['-launch_planet_game-'].update(disabled=False)
        window['-quit_planet_game-'].update(disabled=False)
    if event == '-start_server-':
        taskQueue = Queue()
        responseQueue = Queue()
        server_thread = Thread(target=server.run, args=(taskQueue, responseQueue))
        server_thread.start()
        taskQueue.put(['startServer'])
        window['-start_server-'].update(disabled=True)
        window['-stop_server-'].update(disabled=False)
        window['-calibrate_floor-'].update(disabled=False)
        window['-reset_actuators-'].update(disabled=False)
        window['-stop_actuators-'].update(disabled=False)
    # Kill server thread
    if event == '-stop_planet_server-':
        taskQueue.put(['stopServer'])
        window['-stop_planet_server-'].update(disabled=True)
        window['-start_planet_server-'].update(disabled=False)
        window['-launch_planet_game-'].update(disabled=True)
        window['-quit_planet_game-'].update(disabled=False)
    if event == '-stop_server-':
        taskQueue.put(['stopServer'])
        window['-stop_server-'].update(disabled=True)
        window['-start_server-'].update(disabled=False)
        window['-calibrate_floor-'].update(disabled=True)
        window['-reset_actuators-'].update(disabled=True)
        window['-stop_actuators-'].update(disabled=True)
    if event == '-launch_planet_game-':
        p = subprocess.Popen([root_path+"/../../Starchaeologist.exe"])
    if event == '-quit_planet_game-':
        p.kill()
    if event == '-calibrate_floor-':
        window['-calibrate_floor-'].update(disabled=True)
        taskQueue.put(['calibrateFloor'])
    if event == '-apply_com_port-':
        applyComPort(values['-com_port-'])
    if event in ('-update_com_ports-'):
        updateComPorts()
        window['-com_port-'].update(values=com_ports)
    if event == '-difficulty-':
        gameDifficulty=values['-difficulty-']
    if event == '-applyDifficulty-':
        taskQueue.put(['updateDiff', gameDifficulty])
    if event == '-clearOutput-':
        window.FindElement('-consoleOutput-').Update('')
    if event == '-clearPlot-':
        cpu.clear()
        plt.clf()
        #fig.clf()
        #plt.cla()
        animated_plot.get_tk_widget().forget()
        #plt.close('all')
        ax1 = plt.subplot(2, 1, 1)
        ax2 = plt.subplot(2, 2, 3)
        ax3 = plt.subplot(2, 2, 4)
        fig.tight_layout()
        animated_plot=draw_figure(base_visualization.TKCanvas, fig)
taskQueue.put(['stopServer'])
window.close()
