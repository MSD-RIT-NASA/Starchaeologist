from threading import Event
import threading
import queue
import server_queue_test
import time

q = queue.Queue()

# Turn-on the worker thread.
threading.Thread(target=server_queue_test.run, args=(q, )).start()

# Send thirty task requests to the worker.
q.put(['updateDiff', '0.55'])
q.put(['updateCOM', 'COM7'])
q.put(['killServer'])