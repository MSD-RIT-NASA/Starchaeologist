#!/usr/bin/env python
# -*- coding: utf-8 -*-
# Angela Hudak
# Balance Score calculation demo
# Reading the sensor output of 4 sensors and calculating the Blance Score

import math


#   Convert the kg data to Newtons
#   N = kg * (m/s)^2
#   N = kg * 9.81
def convert_kg_to_N(data):

    for i in range(0, len(data), 1):
        for j in range(4):
            #TODO: Fix / catch occasional out of index error
            if data[i][j] > 0:
                data[i][j] *= 9.81
            else:
                data[i][j] = 0.0
                
    return data


# calculate balance score
def getscore(data):
    ndata = convert_kg_to_N(data)
    score_calc_array = []
    cords = []
    ravg = 0

    # Assume forceplate and sensors are in a square
    # L = length of plate = 0.5381625
    plate_len = 0.5381625

    # clear data.txt of old data
    # TODO: will later change to export data based on profile name and date
    with open('data.txt', 'w') as f:
        f.write("")

    for i in range(0, len(ndata), 1):
        FnegX = ndata[i][0] + ndata[i][2]
        FposX = ndata[i][1] + ndata[i][3]
        FnegY = ndata[i][2] + ndata[i][3]
        FposY = ndata[i][0] + ndata[i][1]

        if (2 * (FposX + FnegX)) == 0 or (2 * (FposY + FnegY)) == 0:
            x = 0
            y = 0
        else:
            x = (plate_len * (FposX - FnegX)) / (2 * (FposX + FnegX))
            y = (plate_len * (FposY - FnegY)) / (2 * (FposY + FnegY))

        r = math.sqrt( ((x*x) + (y*y)) )
        score_calc_array.append(r)
        cords.append(x)
        cords.append(y)
        cords.append(r)
        
        # save coordinate data to txt file for matlab plotting
        with open('data.txt', 'a') as f:
            for item in cords:
                f.write(str(item) + " ")
            f.write("\n")
        cords = []

    # get the average of r
    for j in score_calc_array:  
        ravg += j
    ravg /= len(score_calc_array)

    # calculate ratio of r
    score = 1 - ((2 * ravg) / (math.sqrt(2) * plate_len))

    return score*100


# if __name__ == "__main__":
#     main()