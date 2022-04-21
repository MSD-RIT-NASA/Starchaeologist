// RingBufCPP - Version: Latest 
#include <RingBufCPP.h>
#include <RingBufHelpers.h>

// HX711 - Version: Latest
#include <HX711.h>

/*
  Designed to interface with Python, control four TAS501 sensors (https://www.sparkfun.com/products/14282),
  and communicate the data with a PC over serial connection
  
  There is an operational flowchart in the GitHub (https://github.com/Patrickode/NASA-Balance-Prototype/tree/main/External/Arduino)
  and there are a couple command keys that are defined in forceplateLib.h
*/

typedef struct{
  long timestamp;
  uint8_t sensorNum;
  float measurement;
} Datapoint;

RingBufCPP<Datapoint, 60*15> buf;


HX711 topLft;
HX711 topRgt;
HX711 botLft;
HX711 botRgt;

// Update pin numbers after wiring prototype
// https://docs.arduino.cc/hacking/hardware/PinMapping2560
uint8_t TLdata = 6;
uint8_t TLclk = 7;
uint8_t TRdata = 8;
uint8_t TRclk = 9;
uint8_t BLdata = 10;
uint8_t BLclk = 11;
uint8_t BRdata = 12;
uint8_t BRclk = 13;

//constants command keys
const uint8_t constantMode = 0;
const uint8_t triggerMode = 1;
const uint8_t start = 2;
const uint8_t stop = 4;
const uint8_t sendData = 8;
const uint8_t restart = 128;
const uint8_t hardReset = 255;
const uint8_t calibrateMode = 16;

int mode = 0;
uint8_t pyComm = 0;

  // Maybe get rid of calibration in setup 
  // and create a seperate reset function if calling it multiple times (in setup)
void setup() {
  topLft.begin(TLdata, TLclk);
  topRgt.begin(TRdata, TRclk);
  botLft.begin(BLdata, BLclk);botRgt.begin(BRdata, BRclk);
  
  // Sets the offset variables to be the average readings from the force sensors
  topLft.tare();
  topRgt.tare();
  botLft.tare();
  botRgt.tare();
  
  // Sets a calibration factor to set the factor for linear numbers to be turned into weight numbers 
  topLft.set_scale(420.0983);       // TODO you need to calibrate this yourself.
  topRgt.set_scale(420.0983);       // TODO you need to calibrate this yourself.
  botLft.set_scale(420.0983);       // TODO you need to calibrate this yourself.
  botRgt.set_scale(420.0983);       // TODO you need to calibrate this yourself.
  
  // Tare again
  topLft.tare();
  topRgt.tare();
  botLft.tare();
  botRgt.tare();
  
  // Test the serial connection
  Serial.begin(115200);
  int sync = 0;
  while (sync != 3)
  {
    if (Serial.available() > 0) {
      if (Serial.read() == sync)
        sync++;
    }
  }
  
}

void loop() {
  switch(mode)
  {
    default:
      // blocking wait for a single byte communication from serial connection
      while (Serial.available() == 0)
      {};
      if (Serial.available() > 0) {
        pyComm = Serial.read();
      }
      break;
    
    case constantMode:
      // Constantly records a programmable time frame (default: 90 sec) continously
      // overwriting data in a circular buffer.
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
      
      
      
      
      
      break;
    case triggerMode:
      
      break;
    
    
    
  }

}

void measure(HX711 scale)
{
  Serial.println("Counting get_units() calls for 1 minute...");
  uint32_t count = 0;
  uint32_t start = millis();
  while (millis() - start < 60000)
  {
    if (scale.is_ready())
    {
      count++;
      scale.get_units(1);
    }
  }
  Serial.print("calls per minute: ");
  Serial.println(count);
  Serial.print("calls per second: ");
  Serial.println(count / 60.0);
}
