from pythonosc.osc_server import AsyncIOOSCUDPServer
from pythonosc.dispatcher import Dispatcher
from enum import Enum
import datetime, calendar
import asyncio
import sys
import csv
import time
import os
from threading import Event

cur_line = 0
osc_requests = {}
data = []

script_path = os.path.abspath(__file__)
root_path = os.path.dirname(script_path)
csv_root = root_path+"\\Planet Skeleton Data"

def run(collect: Event, log_data: Event):

    def default_handler(address, *args):
        global cur_line
        global osc_requests
        if address not in osc_requests:
            osc_requests[address] = {
                    "address" : address,
                    "line" : cur_line,
                    "args" : args }
            cur_line += 2
        else:
            osc_requests[address]["args"] = args

    dispatcher = Dispatcher()
    dispatcher.set_default_handler(default_handler)

    ip = "127.0.0.1"
    port = 9000

    if sys.argv == 3:
        try:
            ip = sys.argv[1]
            port = int(sys.argv[2])
        except:
            pass

    async def loop():
        global osc_requests
        global data
        start_time = 0
        started = 0
        while True:
            if collect.is_set():
                if started == 0:
                    start_time = time.time()
                data_collector(osc_requests, start_time)
                started = 1
            if log_data.is_set():
                collect.clear()
                started = 0
                date = datetime.datetime.utcnow()
                end_time = time.time()
                timestamp = str(end_time).split('.')[0]
                time_diff = start_time-end_time
                os.makedirs(csv_root + "\\" + timestamp)

                # Create files
                files = ["chest", "waist", "left_foot", "right_foot", "left_knee", "right_knee"]
                for each in files:
                    file1 = open(csv_root + "/" + timestamp + "/" + each + '_position.csv','a+')
                    file2 = open(csv_root + "/" + timestamp + "/" + each + '_position.csv','w+')
                    file3 = open(csv_root + "/" + timestamp + "/" + each + '_rotation.csv','a+')
                    file4 = open(csv_root + "/" + timestamp + "/" + each + '_rotation.csv','w+')

                for each_dict in data:
                    id_number = each_dict["name"].split("_")[0]
                    data_type = each_dict["name"].split("_")[1]
                    match id_number:
                        case "1":
                            each_dict["name"] = "chest_" + data_type
                        case "2":
                            each_dict["name"] = "waist_" + data_type
                        case "3":
                            each_dict["name"] = "left_foot_" + data_type
                        case "4":
                            each_dict["name"] = "right_foot_" + data_type
                        case "5":
                            each_dict["name"] = "left_knee_" + data_type
                        case "6":
                            each_dict["name"] = "right_knee_" + data_type
                        case _:
                            pass
                    with open(csv_root + "/" + timestamp + "/" + str(each_dict["name"]+".csv"), 'w', newline="") as csvfile:
                        writer = csv.writer(csvfile, delimiter=",")
                        for key in each_dict:
                            if key == "name":
                                continue
                            writer.writerow(each_dict[key])
                log_data.clear()
                data = []
            await asyncio.sleep(0.01)

    def data_collector(requests, start_time):
        meas_time = time.time() - start_time
        for request in requests.values():
            address = request['address']
            address_path = address.split("/")
            args = tuple(request['args'])
            path = str(address_path[3]) + "_" + str(address_path[4])

            # If the dictionary hasn't been made yet for the data we have
            index = next((i for i, item in enumerate(data) if item["name"] == path), None)
            if index is None:
                # Then add the dictionary to the data list and then add the first data set
                data.append(dict(name=path))
                index = next((i for i, item in enumerate(data) if item["name"] == path), None)
            # Add data to dictionary
            data[index][str(meas_time)] = tuple([meas_time]) + args

    async def init_main():
        server = AsyncIOOSCUDPServer((ip, port), dispatcher, asyncio.get_event_loop())
        transport, protocol = await server.create_serve_endpoint()  # Create datagram endpoint and start serving
        
        await loop()  # Enter main loop of program

        transport.close()  # Clean up serve endpoint

    #try:
    asyncio.run(init_main())
  #  except KeyboardInterrupt:
  #      os.makedirs(csv_root + "\\" + str(start_time).split('.')[0])
  #      for each_dict in data:
  #          id_number = each_dict["name"].split("_")[0]
  #          data_type = each_dict["name"].split("_")[1]
  #          match id_number:
  #              case "1":
  #                  each_dict["name"] = "chest_" + data_type
  #              case "2":
  #                  each_dict["name"] = "waist_" + data_type
  #              case "3":
  #                  each_dict["name"] = "left_foot_" + data_type
  #              case "4":
  #                  each_dict["name"] = "right_foot_" + data_type
  #              case "5":
  #                  each_dict["name"] = "left_knee_" + data_type
  #              case "6":
  #                  each_dict["name"] = "right_knee_" + data_type
  #              case _:
  #                  pass
  #          with open(csv_root + "/" + timestamp + "/" + str(each_dict["name"]+".csv"), 'w', newline="") as csvfile:
  #              writer = csv.writer(csvfile, delimiter=",")
  #              for key in each_dict:
  #                  if key == "name":
  #                      continue
  #                  writer.writerow(each_dict[key])