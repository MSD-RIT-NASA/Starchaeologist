import UdpComms as U
import sensors as s
import time
import logging
import socket
import math
import os
from threading import Thread, Event
import planet_matlab, base_matlab, planet_data_collection
import serial
from queue import Queue
import actuator_control

# Actuator control
actuator_taskQueue = Queue()
actuator_responseQueue = Queue()
actuator_thread = Thread(target=actuator_control.run, args=(actuator_taskQueue, actuator_responseQueue))
actuator_thread.start()

time.sleep(7)

print('riverRun')
actuator_taskQueue.put(['riverRun', 0.5])

time.sleep(3)

print('stopActuators')
actuator_taskQueue.put(['stopActuators', 0.0])

time.sleep(3)

print('actuatorCleanup')
actuator_taskQueue.put(['actuatorCleanup', 0.0])

time.sleep(3)

print('actuatorStartup')
actuator_taskQueue.put(['actuatorStartup', 0.0])

time.sleep(3)

print('riverRun')
actuator_taskQueue.put(['riverRun', 0.5])