#ifndef FORCEPLATELIB_H
#define FORCEPLATELIB_H

class forceplateLib
{
  public:
    
  private:
    // Maybe create an array of pointers to these to easily iterate through
    HX711 topLeft;
    HX711 topRight;
    HX711 botLeft;
    HX711 botRight;

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

    //constants for commands from python

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

}

#endif