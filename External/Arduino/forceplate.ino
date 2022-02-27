 // HX711_ADC - Version: Latest 
#include <HX711_ADC.h>

// #include <EEPROM.h>

// const int calVal_eepromAdress_1 = 0; // eeprom adress for calibration value load cell 1 (4 bytes)
// const int calVal_eepromAdress_2 = 4; // eeprom adress for calibration value load cell 2 (4 bytes)

/*
  Designed to interface with Python, control four TAS501 sensors (https://www.sparkfun.com/products/14282),
  and communicate the data with a PC over serial connection
  
  There is an operational flowchart in the GitHub repository
  and there are a couple command keys that are defined in forceplateLib.h
*/

typedef struct{
  long timestamp;
  float measurements[4];
  // float measurement1;
  // float measurement2;
  // float measurement3;
  // float measurement4;
} Datapoint;

// Datapoint data[1200];

// Update pin numbers after wiring prototype
// https://docs.arduino.cc/hacking/hardware/PinMapping2560
const uint8_t HX711_dout_1 = 30;
const uint8_t HX711_sck_1 = 31;
const uint8_t HX711_dout_2 = 34;
const uint8_t HX711_sck_2 = 35;
const uint8_t HX711_dout_3 = 38;
const uint8_t HX711_sck_3 = 39;
const uint8_t HX711_dout_4 = 44;
const uint8_t HX711_sck_4 = 45;

HX711_ADC LoadCell_1(HX711_dout_1, HX711_sck_1); // Top left
HX711_ADC LoadCell_2(HX711_dout_2, HX711_sck_2); // Top right
HX711_ADC LoadCell_3(HX711_dout_3, HX711_sck_3); // Bot left
HX711_ADC LoadCell_4(HX711_dout_4, HX711_sck_4); // Bot right

//constants command keys
const uint8_t wait = 0;
const uint8_t triggerMode = 1;
const uint8_t constantMode = 3;
const uint8_t start = 2;
const uint8_t stop = 4;
const uint8_t sendData = 8;
const uint8_t restart = 128;
const uint8_t hardReset = 255;
const uint8_t calibrateMode = 16;

int mode = 0;
uint8_t pyComm = 0;

void setup() {
  
  Serial.begin(115200);
  delay(10);
  Serial.println();
  Serial.println("serial connection made");
  
  float calibrationValue_1 = 696.0;
  float calibrationValue_2 = 696.0;
  float calibrationValue_3 = 696.0;
  float calibrationValue_4 = 696.0;
 
  uint8_t gain = 128; 
  LoadCell_1.begin(gain);
  LoadCell_2.begin(gain);
  LoadCell_3.begin(gain);
  LoadCell_4.begin(gain);
  
  unsigned long stabilizingtime = 2000; // tare preciscion can be improved by adding a few seconds of stabilizing time
  boolean _tare = true; //set this to false if you don't want tare to be performed in the next step
  byte loadcell_1_rdy = 0;
  byte loadcell_2_rdy = 0;
  byte loadcell_3_rdy = 0;
  byte loadcell_4_rdy = 0;
  while ((loadcell_1_rdy + loadcell_2_rdy + loadcell_3_rdy + loadcell_4_rdy) < 4) { //run startup, stabilization and tare, both modules simultaniously
    if (!loadcell_1_rdy) loadcell_1_rdy = LoadCell_1.startMultiple(stabilizingtime, _tare);
    if (!loadcell_2_rdy) loadcell_2_rdy = LoadCell_2.startMultiple(stabilizingtime, _tare);
    if (!loadcell_3_rdy) loadcell_3_rdy = LoadCell_3.startMultiple(stabilizingtime, _tare);
    if (!loadcell_4_rdy) loadcell_4_rdy = LoadCell_4.startMultiple(stabilizingtime, _tare);
  }
  if (LoadCell_1.getTareTimeoutFlag()) {
    Serial.println("Timeout, check MCU>HX711 no.1 wiring and pin designations");
  }
  if (LoadCell_2.getTareTimeoutFlag()) {
    Serial.println("Timeout, check MCU>HX711 no.2 wiring and pin designations");
  }
  if (LoadCell_3.getTareTimeoutFlag()) {
    Serial.println("Timeout, check MCU>HX711 no.3 wiring and pin designations");
  }
  if (LoadCell_4.getTareTimeoutFlag()) {
    Serial.println("Timeout, check MCU>HX711 no.4 wiring and pin designations");
  }
  LoadCell_1.setCalFactor(calibrationValue_1);
  LoadCell_2.setCalFactor(calibrationValue_2);
  LoadCell_3.setCalFactor(calibrationValue_3);
  LoadCell_4.setCalFactor(calibrationValue_4);
  LoadCell_1.setSamplesInUse(1);
  LoadCell_2.setSamplesInUse(1);
  LoadCell_3.setSamplesInUse(1);
  LoadCell_4.setSamplesInUse(1);
  Serial.println("Startup is complete");
  mode = wait;
}

void loop() {
  switch(mode)
  {
    default:
    {
      mode = wait;
    } break;
    
    case wait:
    {
      // blocking wait for a single byte communication from serial connection
      Serial.println("Waiting");
      while (!Serial.available());
      pyComm = Serial.read();
      if(pyComm == constantMode)
      {
        mode = pyComm;
      }
    } break;
    
    case constantMode:
    {
      // After collecting data of one recording cycle (records all four sensors and
      // the millisecond it was taken (from the time the arduino was turned on)),
      // it checks serial for the command to send the data from the last time frame.
      // When the command arrives, it should break loop and send data over serial 
      // or go to a different mode of operation.
      // If data is sent successfully, it will then go back to continously recording.
      
      // Uncomment and edit to add option for secondary command to set time frame in sec
      // while (Serial.available() == 0)
      // {};
      // if (Serial.available() > 0) {
      //   pyComm = Serial.read();
      // }
      int secsToRemember = 60;
      int index = 0;
      int maxIndex = secsToRemember*10;  // might get rid of this variable
      unsigned long t = 0;
      Datapoint data[1200];
      bool go = true;
      Serial.println("Starting to record");
      t = millis();
      while(go)
      {
        static boolean newDataReady = 0;
        
        // check for new data/start next conversion:
        if (LoadCell_1.update()) newDataReady = true;
        LoadCell_2.update();
        LoadCell_3.update();
        LoadCell_4.update();
        
        if ((newDataReady)) {
          data[index].timestamp = millis();
           data[index].measurements[0] = LoadCell_1.getData();
           data[index].measurements[1] = LoadCell_2.getData();
           data[index].measurements[2] = LoadCell_3.getData();
           data[index].measurements[3] = LoadCell_4.getData();
          
          newDataReady = 0;
          
          index++;
          
          // Update index and maxIndex
          if (millis() > t + secsToRemember*1000) {
            t = millis();
          }
          
        }
      
        // check serial to send data or to exit this mode
        if (Serial.available() > 0) {
          pyComm = Serial.read();
          if (pyComm == sendData) {
            for(int i = 0; i < maxIndex; i++)
            {
              Serial.write(data[index].timestamp);
              Serial.write(data[index].measurements, 16);
            }
          }
          else
          {
            mode = pyComm;
          }
        }
      }
      
    } break;
    case triggerMode:
    { 
      
    } break;
  }
}

void serial_flush() {
  while (Serial.available()) Serial.read();
}
