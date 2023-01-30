#include "Arduino.h"
#include "HX711.h"

#define SCALE_1_PIN_DAT 50
#define SCALE_1_PIN_CLK 48
#define SCALE_2_PIN_DAT 46
#define SCALE_2_PIN_CLK 44
#define SCALE_3_PIN_DAT 42
#define SCALE_3_PIN_CLK 40
#define SCALE_4_PIN_DAT 38
#define SCALE_4_PIN_CLK 36


//typedef struct{
//  long timestamp;
//  long measurements[4];
//} Datapoint;

HX711 scale_1(SCALE_1_PIN_DAT, SCALE_1_PIN_CLK);
#define calibration_factor 2280 //This value is obtained using the SparkFun_HX711_Calibration sketch https://learn.sparkfun.com/tutorials/load-cell-amplifier-hx711-breakout-hookup-guide?_ga=2.77038550.2126325781.1526891300-303225217.1493631967
HX711 scale_2(SCALE_2_PIN_DAT, SCALE_2_PIN_CLK);
#define calibration_factor 2280 //This value is obtained using the SparkFun_HX711_Calibration sketch https://learn.sparkfun.com/tutorials/load-cell-amplifier-hx711-breakout-hookup-guide?_ga=2.77038550.2126325781.1526891300-303225217.1493631967
HX711 scale_3(SCALE_3_PIN_DAT, SCALE_3_PIN_CLK);
#define calibration_factor 2280 //This value is obtained using the SparkFun_HX711_Calibration sketch https://learn.sparkfun.com/tutorials/load-cell-amplifier-hx711-breakout-hookup-guide?_ga=2.77038550.2126325781.1526891300-303225217.1493631967
HX711 scale_4(SCALE_4_PIN_DAT, SCALE_4_PIN_CLK);
#define calibration_factor 2280 //This value is obtained using the SparkFun_HX711_Calibration sketch https://learn.sparkfun.com/tutorials/load-cell-amplifier-hx711-breakout-hookup-guide?_ga=2.77038550.2126325781.1526891300-303225217.1493631967

const int timeout = 10000;       //define timeout of 10 sec

int mode = 0;
String pyComm = " ";
int j = 0;
//constants command keys
const int wait = 0;
const int triggerMode = 1;
const int constantMode = 3;
const int startRead = 2;
const int sendData = 8;
const int restart = 128;
const int hardReset = 255;
const int calibrateMode = 16;
const String endSend = "END";
long scale_1Units = 0;
long scale_2Units = 0;
long scale_3Units = 0;
long scale_4Units = 0;

void setup() {
  Serial.begin(115200);
  while (!Serial) ;
//  Serial.println();
  Serial.println("serial connection made");
  scale_1.set_scale(calibration_factor); 
  scale_1.tare(); //Assuming there is no weight on the scale at start up, reset the scale to 0
  scale_2.set_scale(calibration_factor); 
  scale_2.tare(); //Assuming there is no weight on the scale at start up, reset the scale to 0
  scale_3.set_scale(calibration_factor); 
  scale_3.tare(); //Assuming there is no weight on the scale at start up, reset the scale to 0
  scale_4.set_scale(calibration_factor); 
  scale_4.tare(); //Assuming there is no weight on the scale at start up, reset the scale to 0
  
//  mode = wait;
}

void loop() {
  Serial.println(mode);
  switch(mode)
  {
    
    case wait:
    {
      // blocking wait for a single byte communication from serial connection
      Serial.println("Waiting");
      while (!Serial.available());
      Serial.println("Done Waiting ");
      pyComm = Serial.readString();
      Serial.println(pyComm);
      if (pyComm.toInt() == constantMode or pyComm == "3"){
        mode = pyComm.toInt();  
      }
      break;
    } 
    case constantMode:{
      Serial.flush();
      while(!Serial.available());
      pyComm = Serial.readString();
      if (pyComm.toInt() == sendData or pyComm == "8"){
        Serial.println("Starting to record");
        int start = millis(); 
        int count = 0;
        while(start + 5000 > millis()){
          Serial.println(millis());
          scale_1Units = scale_1.read(); //scale.get_units() returns a float
//          scale_1Units = count;
          Serial.println(String(scale_1Units)); //You can change this to lbs but you'll need to refactor the calibration_factor
          count++;
          scale_2Units = scale_2.read(); //scale.get_units() returns a float
//          scale_2Units = count;
          Serial.println(String(scale_2Units)); //You can change this to lbs but you'll need to refactor the calibration_factor
          count++;
          scale_3Units = scale_3.read(); //scale.get_units() returns a float
//          scale_3Units = count;
          Serial.println(String(scale_3Units)); //You can change this to lbs but you'll need to refactor the calibration_factor
          count++;
          scale_4Units = scale_4.read(); //scale.get_units() returns a float
//          scale_4Units = count;
          count++;
          Serial.println(String(scale_4Units)); //You can change this to lbs but you'll need to refactor the calibration_factor
          Serial.println("1");
        }
        Serial.println("END");
      } else {
        mode = wait;
      }
      break;
    } 
    case triggerMode:
    { 
      break;
    } 
    default:{
      mode = wait;
      break;
    } 
  }

}