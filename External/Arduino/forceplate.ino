// HX711 - Version: Latest
#include <HX711.h>
//#include "forceplateLib.h"

/*
  THIS CODE IS FOR IMPLEMENTATION
  TODO: MAKE A SKETCH FOR TESTING FOR SCALE CALIBRATION FACTORS
  
  Designed to interface with Python, control four TAS501 sensors (https://www.sparkfun.com/products/14282),
  and communicate the data with a PC over serial connection
  
  There is an operational flowchart in the GitHub (https://github.com/Patrickode/NASA-Balance-Prototype/tree/main/External/Arduino)
  and there are a couple command keys that are defined in forceplateLib.h
*/


HX711 topLft;
HX711 topRgt;
HX711 botLft;
HX711 botRgt;
volatile float f;

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
const uint8_t restart = 10;
// Add a constant for a hard reset
// Add a constant for calibrate 
  // Maybe get rid of calibration in setup 
  // and create a seperate reset function if calling it multiple times (in setup)

void setup() {
  topLft.begin(TLdata, TLclk);
  topRgt.begin(TRdata, TRclk);
  botLft.begin(BLdata, BLclk);
  botRgt.begin(BRdata, BRclk);
  
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
  uint8_t pyComm = 0;
  while (Serial.available() == 0);
  if (Serial.available() > 0) {
    pyComm = Serial.read();
  }
  
  if (pyComm == constantMode)
  {
    measure(topLft);
  }
  else if (pyComm == triggerMode)
  {
    measure(topLft);
  }
  else
  {
    // serial send error?
    measure(topLft);
  }


  // read all four sensors

  // send all four sensors



  // mode 1
  // constantly measuring, only keeps last 1 min. or so until   comm.
  // maybe makes another command for putting it in either mode
  // options for length of time (30 s, 60 s)
  //

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
