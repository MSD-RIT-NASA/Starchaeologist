
#include "Arduino.h"
#include "HX711.h"

/*
  Designed to interface with Python, control four TAS501 sensors (https://www.sparkfun.com/products/14282),
  and communicate the data with a PC over serial connection
*/

typedef struct{
  long timestamp;
  long measurements[4];
} Datapoint;

#define SCALE_1_PIN_DAT 50
#define SCALE_1_PIN_CLK 48
#define SCALE_2_PIN_DAT 46
#define SCALE_2_PIN_CLK 44
#define SCALE_3_PIN_DAT 42
#define SCALE_3_PIN_CLK 40
#define SCALE_4_PIN_DAT 38
#define SCALE_4_PIN_CLK 36

#define calibration_factor 2280

HX711 scale_1(SCALE_1_PIN_DAT, SCALE_1_PIN_CLK);
HX711 scale_2(SCALE_2_PIN_DAT, SCALE_2_PIN_CLK);
HX711 scale_3(SCALE_3_PIN_DAT, SCALE_3_PIN_CLK);
HX711 scale_4(SCALE_4_PIN_DAT, SCALE_4_PIN_CLK);

//constants command keys
const uint8_t wait = 0;
const uint8_t sendAsYouGoMode = 1;
const uint8_t constantMode = 3;
const uint8_t sendData = 8;
const uint8_t calibrateMode = 16;   // Implment this to run the tare() at the right time
const uint8_t endSend = 42;

uint8_t mode = 0;
uint8_t pyComm = 0;

void setup() {
  Serial.begin(115200);
  while(!Serial);   // waits for laptop serial port to connect
  // Serial.println("serial connection made");
  
  scale_1.set_scale(calibration_factor);
  scale_2.set_scale(calibration_factor);
  scale_3.set_scale(calibration_factor);
  scale_4.set_scale(calibration_factor);
  scale_1.tare();
  scale_2.tare();
  scale_3.tare();
  scale_4.tare();
  
  Serial.println("Startup is complete");
  mode = wait;
}

void loop() 
{
  switch (mode) 
  {
    case wait:
    {
      // blocking wait for communication from serial connection
      Serial.println("Waiting");
      Serial.flush();
      while (!Serial.available());
      pyComm = Serial.read();
      if (pyComm == constantMode) {
        mode = pyComm;
      }
      break;
    }

    case constantMode:
    {
      // After collecting data of one recording cycle (records all four sensors and
      // the millisecond it was taken (from the time the arduino was turned on)),
      // it checks serial for the command to send the data from the last time frame.
      // When the command arrives, it should break loop and send data over serial
      // or go to a different mode of operation.
      // If data is sent successfully, it will then go back to waiting mode.

      // Uncomment and edit to add option for secondary command to set time frame in sec
      // while (Serial.available() == 0)
      // {};
      // if (Serial.available() > 0) {
      //   secsToRemember = Serial.read();
      // }
      unsigned int secsToRemember = 60;
      int index = 1200;
      int timeIndex = 0;
      Datapoint data[1200];
      bool go = true;
      // Serial.println("Starting to record");
      
      while(go)
      {
        // Add next datapoint
        index++;
        if (index == 1200) 
        {
          index = 0;
        }
        
        data[index].timestamp = millis();
        data[index].measurements[0] = scale_1.read();
        data[index].measurements[1] = scale_2.read();
        data[index].measurements[2] = scale_3.read();
        data[index].measurements[3] = scale_4.read();

        // check serial to send data or to switch modes
        if(Serial.available() > 0) 
        {
          pyComm = Serial.read();
          if(pyComm == sendData) 
          {
            // This sets timeIndex to the first index of the last x seconds of data
            bool search = true;
            timeIndex = index;
            while(search)
            {
              timeIndex--;
              if(timeIndex < 0) 
              {
                timeIndex = 1199;
              }

              if(secsToRemember * 1000 < data[index].timestamp - data[timeIndex].timestamp) 
              {
                search = false;
              }
              // if(timeIndex == index)
              // {
              // Serial.println("Has not recorded selected amount of time yet. Pls let it run longer before reading");
              // search = false;
              // }
               // maybe put in some safety measures in case the search messes up?
            }

            for(int i = timeIndex; i != index + 1; i++)
            {
              if(i == 1200) 
              {
                i = 0;
              }
              Serial.println(String(data[i].measurements[0]));
              Serial.println(String(data[i].measurements[1]));
              Serial.println(String(data[i].measurements[2]));
              Serial.println(String(data[i].measurements[3]));
              Serial.flush();
            }
            Serial.write(endSend);
            Serial.flush();
            go = false; // comment if should stay in this mode 
          } 
          else
          {
            mode = pyComm;
            go = false;
          }
        }
      }
      break;
    }

    case sendAsYouGoMode:
    {
      unsigned int secsToRemember = 60;
      bool go = true;
      unsigned long t = millis();
      while(go) 
      {
        Serial.println(String(scale_1.read()));
        Serial.println(String(scale_2.read()));
        Serial.println(String(scale_3.read()));
        Serial.println(String(scale_4.read()));
        Serial.flush();
        
        // check if time is up
        if (secsToRemember * 1000 > millis() - t) 
        {
          go = false;
        }
      }
      Serial.write(endSend);
      mode = wait;
      break;
    }

    default:
    {
      mode = wait;
      break;
    }
  }
}

void serial_flush() 
{
  while (Serial.available()) Serial.read();
}
