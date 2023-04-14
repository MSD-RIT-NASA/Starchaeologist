from pycomm3 import LogixDriver
from enum import Enum
import random
import numpy as np
import time
import math
from sklearn.preprocessing import minmax_scale
import threading
import time
from threading import Event
from queue import Queue

IP = "10.100.20.10"
LEVEL_POS = -12500
ACC = 45000
ACTUATOR_VAR = 5
DEBUG = 1

MIN_SPEED = 2500
MIN_LOWER_SPAN = -15000
MIN_UPPER_SPAN = -10000

MAX_SPEED = 30000
MAX_LOWER_SPAN = -24500
MAX_UPPER_SPAN = -500


class ACT(Enum):
    RIGHT = 1
    LEFT = 2
    BOTH = 3

def actuator_move(speed_right: int, acc_right: int, pos_right: int, speed_left: int, acc_left: int, pos_left: int):

    if speed_right not in range(1, 30000+1) or speed_left not in range(1, 30000+1):
        raise Exception("Speed value invalid, must be between 1 and 30000")
    elif acc_right not in range(500, 90000+1) or acc_left not in range(500, 90000+1):
        raise Exception("Acceleration value invalid, must be between 500 and 90000")
    elif pos_right not in range(-25000, 0+1) or pos_left not in range(-25000, 0+1):
        raise Exception("Position value invalid, must be between -25000 and 0")

    with LogixDriver(IP) as plc:
        plc.write(('_Right_moveSpeed', speed_right), ('_Right_moveAccel', acc_right), ('_Right_movePosition', pos_right), ('Cmd_Right_Move', pos_right),
                  ('_Left_moveSpeed', speed_left), ('_Left_moveAccel', acc_left), ('_Left_movePosition', pos_left),  ('Cmd_Left_Move', pos_left))


def actuator_single_move(actuator: ACT, speed: int, acc: int, position: int):

    if speed not in range(1, 30000+1):
        raise Exception("Speed value invalid, must be between 1 and 30000")
    elif acc not in range(500, 90000+1):
        raise Exception("Acceleration value invalid, must be between 500 and 90000")
    elif position not in range(-25000, 0+1):
        raise Exception("Position value invalid, must be between -25000 and 0")

    with LogixDriver(IP) as plc:
        if actuator == ACT.LEFT:
            plc.write(('_Left_moveSpeed', speed), ('_Left_moveAccel', acc), ('_Left_movePosition', position), ('Cmd_Left_Move', 1))
        elif actuator == ACT.RIGHT:
            plc.write(('_Right_moveSpeed', speed), ('_Right_moveAccel', acc), ('_Right_movePosition', position), ('Cmd_Right_Move', 1))
        elif actuator == ACT.BOTH:
            plc.write(('_Left_moveSpeed', speed), ('_Left_moveAccel', acc), ('_Left_movePosition', position), ('Cmd_Left_Move', 1),
                      ('_Right_moveSpeed', speed), ('_Right_moveAccel', acc), ('_Right_movePosition', position), ('Cmd_Right_Move', 1))


def actuator_on(actuator: ACT):

    with LogixDriver(IP) as plc:
        if actuator == ACT.RIGHT:
            plc.write(('Cmd_Right_MSO', 1))
        elif actuator == ACT.LEFT:
            plc.write(('Cmd_Left_MSO', 1))
        elif actuator == ACT.BOTH:
            plc.write(('Cmd_Right_MSO', 1))
            plc.write(('Cmd_Left_MSO', 1))

def actuator_off(actuator: ACT):

    with LogixDriver(IP) as plc:
        if actuator == ACT.RIGHT:
            plc.write(('Cmd_Right_MSF', 1))
        elif actuator == ACT.LEFT:
            plc.write(('Cmd_Left_MSF', 1))
        elif actuator == ACT.BOTH:
            plc.write(('Cmd_Right_MSF', 1))
            plc.write(('Cmd_Left_MSF', 1))

# NEVER USE THE STOP FLAG
def actuator_stop(actuator: ACT):

    with LogixDriver(IP) as plc:
        if actuator == ACT.RIGHT:
            plc.write(('Cmd_Right_MAS', 1))
        elif actuator == ACT.LEFT:
            plc.write(('Cmd_Left_MAS', 1))
        elif actuator == ACT.BOTH:
            plc.write(('Cmd_Right_MAS', 1))
            plc.write(('Cmd_Left_MAS', 1))


def actuator_home(actuator: ACT):

    with LogixDriver(IP) as plc:
        if actuator == ACT.RIGHT:
            plc.write(('Cmd_Right_MAH', 1))
        elif actuator == ACT.LEFT:
            plc.write(('Cmd_Left_MAH', 1))
        elif actuator == ACT.BOTH:
            plc.write(('Cmd_Right_MAH', 1))
            plc.write(('Cmd_Left_MAH', 1))


def actuator_reset(actuator: ACT):

    with LogixDriver(IP) as plc:
        if actuator == ACT.RIGHT:
            plc.write('Cmd_Right_MAFR', 1)
        elif actuator == ACT.LEFT:
            plc.write(('Cmd_Left_MAFR', 1))
        elif actuator == ACT.BOTH:
            plc.write(('Cmd_Right_MAFR', 1))
            plc.write(('Cmd_Left_MAFR', 1))


def actuator_level(speed=15000, acc=45000):
    actuator_move(speed, acc, LEVEL_POS, speed, acc, LEVEL_POS)
    level = False
    while not level:
        actuator_move(speed, acc, LEVEL_POS, speed, acc, LEVEL_POS)
        if abs(LEVEL_POS - get_position(ACT.RIGHT)) <= ACTUATOR_VAR and abs(LEVEL_POS - get_position(ACT.LEFT)) <= ACTUATOR_VAR:
            level = True


def actuator_wave(diff_scale, duration):
    diff_params = wave_difficulty(diff_scale)
    pos = np.linspace(diff_params[1], diff_params[2], 6)
    t_end = time.time() + duration
    while time.time() < t_end:
        for i in range(0, int(len(pos)), 2):
            actuator_move(diff_params[0], int(diff_params[0]*(1+diff_scale)), int(pos[i]), diff_params[0], int(diff_params[0]*(1+diff_scale)), int(pos[i+1]))


def wave_difficulty(difficulty):
    u = np.array([MIN_SPEED, MIN_LOWER_SPAN, MIN_UPPER_SPAN])
    v = np.array([MAX_SPEED-MIN_SPEED, MAX_LOWER_SPAN-MIN_LOWER_SPAN, MAX_UPPER_SPAN-MIN_UPPER_SPAN])
    return np.add(u, np.dot(difficulty, v))


def actuator_random(speed, acc, span, duration):
    t_end = time.time() + duration
    while time.time() < t_end:
        actuator_move(speed, acc, int(random.uniform(span[0], span[1])), speed, acc, int(random.uniform(span[0], span[1])))


def get_position(actuator):
    with LogixDriver(IP) as plc:
        if actuator == ACT.RIGHT:
            return plc.read('Right_ActualPosition').value
        elif actuator == ACT.LEFT:
            return plc.read('Left_ActualPosition').value


def get_velocity(actuator):
    with LogixDriver(IP) as plc:
        if actuator == ACT.RIGHT:
            return plc.read('Right_ActualVelocity').value
        elif actuator == ACT.LEFT:
            return plc.read('Left_ActualVelocity').value


def oscillate(freq1, freq2, active, stop, diff=0.5):

    counter = 0
    left_position = LEVEL_POS
    right_position = LEVEL_POS
    min_pos = MIN_LOWER_SPAN + (diff * (MAX_LOWER_SPAN - MIN_LOWER_SPAN))
    max_pos = MIN_UPPER_SPAN + (diff * (MAX_UPPER_SPAN - MIN_UPPER_SPAN))
    while not active.is_set():
        print("WAITING")
        continue
    while True:
        print("GOING")
        print(counter)
        counter = counter + 1

        if freq1 < freq2:
            right_speed = int(MAX_SPEED * diff)
            left_speed = int(MAX_SPEED * diff * (freq1 / freq2))
        else:
            right_speed = int(MAX_SPEED * diff * (freq2 / freq1))
            left_speed = int(MAX_SPEED * diff)

        if right_position == max_pos:
            right_position = min_pos
        else:
            right_position = max_pos
        if left_position == max_pos:
            left_position = min_pos
        else:
            left_position = max_pos
        print("Left, Right", left_position, right_position)
        actuator_move(right_speed, ACC, right_position, left_speed, ACC, left_position)
        if stop.is_set():
            print("STOPPING")
            return

def actuator_sinewave(freq1, freq2, duration):

    sleep_amount = 0.01
    left_position = 0
    right_position = 0

    # Calculate minimal number of steps needed to represent both waves
    steps = math.lcm(int(freq1 * 4), int(freq2 * 4)) + 1

    # Create linear space array for the sin wave generation
    x = np.linspace(0, 2 * math.pi, num=steps)

    # Create sine waves
    out = np.sin(freq1 * x)
    out2 = np.sin(freq2 * x + math.pi)

    # Scale waves to min and max position values
    out = MAX_LOWER_SPAN + (out * (MAX_UPPER_SPAN - MAX_LOWER_SPAN))
    out2 = MAX_LOWER_SPAN + (out2 * (MAX_UPPER_SPAN - MAX_LOWER_SPAN))

    speed = 30000
    acc = 45000
    right_speed = speed
    left_speed = speed
    right_acc = acc
    left_acc = acc

    # Start timer for run duration
    t_end = time.time() + duration
    while time.time() < t_end:
        # Iterate through entire wave
        for i in range(0, steps - 1):
            time.sleep(sleep_amount)
            # If current value in wave is min, max or level, then move the actuator
            if int(out[i]) in (MAX_LOWER_SPAN, MAX_UPPER_SPAN):
                right_position = int(out[i])
            if int(out2[i]) in (MAX_LOWER_SPAN, MAX_UPPER_SPAN):
                left_position = int(out2[i])
            print("Moving left,right to ", left_position, right_position)
            actuator_move(speed, acc, right_position, speed, acc, left_position)


def actuator_startup():
    actuator_on(ACT.LEFT)
    actuator_on(ACT.RIGHT)
    actuator_move(30000, 45000, LEVEL_POS, 30000, 45000, LEVEL_POS)
    time.sleep(1)
    left_position = get_position(ACT.LEFT)
    right_position = get_position(ACT.RIGHT)
    if abs(-LEVEL_POS + left_position) > ACTUATOR_VAR:
        actuator_home(ACT.LEFT)
        if DEBUG:
            print("Sending ACT.LEFT to HOME.")
    if abs(-LEVEL_POS + right_position) > ACTUATOR_VAR:
        actuator_home(ACT.RIGHT)
        if DEBUG:
            print("Sending ACT.RIGHT to HOME.")
    actuator_level()


def actuator_cleanup():
    time.sleep(0.25)
    actuator_off(ACT.LEFT)
    actuator_off(ACT.RIGHT)
    time.sleep(0.25)
    actuator_on(ACT.LEFT)
    actuator_on(ACT.RIGHT)
    time.sleep(0.25)
    actuator_level()
    time.sleep(0.25)
    actuator_off(ACT.LEFT)
    actuator_off(ACT.RIGHT)


def loop(riverRun: Event, puzzlingTimes: Event, active:Event, stop: Event):
    while True:
        if riverRun.is_set():
            print("AGANE")
            oscillate(2, 1, active, stop)
        elif puzzlingTimes.is_set():
            pass
        if stop.is_set():
            riverRun.clear()
            active.clear()
            stop.clear()
            actuator_cleanup()
            break


def run(riverRun: Event, puzzlingTimes: Event, active: Event, stop: Event):
    try:
        while True:
            actuator_startup()
            loop(riverRun, puzzlingTimes, active, stop)
            actuator_cleanup()
    except Exception as e:
        actuator_cleanup()