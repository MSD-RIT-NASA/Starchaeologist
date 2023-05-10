from threading import Event
from queue import Queue

def updateCOM(comPort):
    print(f'Updating COM to {comPort}')
    print(f'Finished updating COM to {comPort}')

def updateDiff(difficulty):
    print(f'Updating diff to {difficulty}')
    print(f'Finished updating diff to {difficulty}')

def run(q: Queue):
    message = []
    while True:
        if not q.empty():
            message = q.get()
            if message[0] == "updateCOM":
                updateCOM(message[1])
            if message[0] == "updateDiff":
                updateDiff(message[1])
            if message[0] == "killServer":
                break