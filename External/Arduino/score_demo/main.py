#!/usr/bin/env python
# -*- coding: utf-8 -*-
# Angela Hudak
# Balance Score calculation demo
# Reading the sensor output of 4 sensors and calculating the Blance Score

import logging
import serial
import time

# Main funtion to receive data
def main():
    """ Main program """
    
    # # set up the serial line
    # ser = serial.Serial('COM10', 9600)
    # time.sleep(2)

    # show the data
    data = getdata()
    for line in data:
        print(line)
    
    #balance_score = getscore(data)
    #print("Final score: " + balance_score)

    return 0


# Receive game data
def getdata():
    
    # move this evenutally to main
    # set up the serial line
    ser = serial.Serial('COM10', 9600)
    time.sleep(2)

    """
        Grab sensor data from the arduino
    """
    logging.info("Started Gathering Info From Force Platform")

    balanceData = []
    dataEntry = []
    # dataSet = False
    for i in range(3000):
        #while True:
                data = ser.readline().decode("ISO-8859-1").strip()                  # read a byte string
                if data == "END":
                    print(i+1)
                    balanceData.append(dataEntry)
                    dataEntry = []
                else:
                    dataEntry.append(float(data))
    logging.info("Gathered Balance Data")

    ser.close()
    return balanceData


    # # Read and record the data
    # data =[]                       # empty list to store the data
    # for i in range(50):
    #     b = ser.readline()         # read a byte string
    #     string_n = b.decode()      # decode byte string into Unicode  
    #     string = string_n.rstrip() # remove \n and \r
    #     flt = float(string)        # convert string to float
    #     print(flt)
    #     data.append(flt)           # add to the end of data list
    #     time.sleep(0.1)            # wait (sleep) 0.1 seconds
        
    #ser.close()

    #return data

# calculate blanace score
def getscore(data):


   return 0

if __name__ == "__main__":
    main()