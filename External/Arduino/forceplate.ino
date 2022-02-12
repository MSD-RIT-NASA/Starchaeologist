// HX711 - Version: Latest
#include <HX711.h>
#include "forceplateLib.h"

/*
  Deisgned to interface with Python, control 
*/


volatile float f;

void setup() {
  topLft.begin(TLdata, TLclk);
  topRgt.begin(TRdata, TRclk);
  botLft.begin(BLdata, BLclk);
  botRgt.begin(BRdata, BRclk);
  
  // 
  topLft.tare();
  topRgt.tare();
  botLft.tare();
  botRgt.tare();

  topLft.set_scale(420.0983);       // TODO you need to calibrate this yourself.
  topRgt.set_scale(420.0983);       // TODO you need to calibrate this yourself.
  botLft.set_scale(420.0983);       // TODO you need to calibrate this yourself.
  botRgt.set_scale(420.0983);       // TODO you need to calibrate this yourself.
  
  
  // 
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
