/*
 Example using the SparkFun HX711 breakout board with a scale
 By: Nathan Seidle
 SparkFun Electronics
 Date: November 19th, 2014
 License: This code is public domain but you buy me a beer if you use this and we meet someday (Beerware license).
 
 This example demonstrates basic scale output. See the calibration sketch to get the calibration_factor for your
 specific load cell setup.
 
 This example code uses bogde's excellent library: https://github.com/bogde/HX711
 bogde's library is released under a GNU GENERAL PUBLIC LICENSE
 
 The HX711 does one thing well: read load cells. The breakout board is compatible with any wheat-stone bridge
 based load cell which should allow a user to measure everything from a few grams to tens of tons.

 Arduino pin 2 -> HX711 CLK
 3 -> DAT
 5V -> VCC
 GND -> GND
 
 The HX711 board can be powered from 2.7V to 5V so the Arduino 5V power should be fine.
 
*/



// Score Demo got GPBA
// author: Angela Hudak
// Description: To send sensor data to be read and manuplulated by a python script for a balance score.

#include "HX711.h"           // This library can be obtained here http://librarymanager/All#Avia_HX711

//-21350 for sensor 1        // This value is obtained using the SparkFun_HX711_Calibration sketch
//-20850 for sensor 2        // This value is obtained using the SparkFun_HX711_Calibration sketch
//-22700 for sensor 3 and 4  // This value is obtained using the SparkFun_HX711_Calibration sketch

#define Sensor1_DOUT_PIN 50
#define Sensor1_SCK_PIN 48

#define Sensor2_DOUT_PIN 46
#define Sensor2_SCK_PIN 44

#define Sensor3_DOUT_PIN 51
#define Sensor3_SCK_PIN 49

#define Sensor4_DOUT_PIN 47
#define Sensor4_SCK_PIN 45

HX711 scale_sensor1;
HX711 scale_sensor2;
HX711 scale_sensor3;
HX711 scale_sensor4;

void setup() {
  Serial.begin(9600);
  //Serial.println("HX711 scale demo");

  scale_sensor1.begin(Sensor1_DOUT_PIN, Sensor1_SCK_PIN);
  scale_sensor2.begin(Sensor2_DOUT_PIN, Sensor2_SCK_PIN);
  scale_sensor3.begin(Sensor3_DOUT_PIN, Sensor3_SCK_PIN);
  scale_sensor4.begin(Sensor4_DOUT_PIN, Sensor4_SCK_PIN);

  scale_sensor1.set_scale(-21350); //This value is obtained by using the SparkFun_HX711_Calibration sketch
  scale_sensor2.set_scale(-20850); //This value is obtained by using the SparkFun_HX711_Calibration sketch
  scale_sensor3.set_scale(-22700); //This value is obtained by using the SparkFun_HX711_Calibration sketch
  scale_sensor4.set_scale(-22700); //This value is obtained by using the SparkFun_HX711_Calibration sketch

  scale_sensor1.tare();  //Assuming there is no weight on the scale at start up, reset the scale to 0
  scale_sensor2.tare();  //Assuming there is no weight on the scale at start up, reset the scale to 0
  scale_sensor3.tare();  //Assuming there is no weight on the scale at start up, reset the scale to 0
  scale_sensor4.tare();  //Assuming there is no weight on the scale at start up, reset the scale to 0

  String myCmd = "";
  Serial.println("Calibration completed");
  while(Serial.available() == 0){}
  myCmd = Serial.readString();
  while(myCmd != "y"){}
}

void loop() {
  
  //Serial.print("Sensor 1: ");
  Serial.println(scale_sensor1.get_units(), 1); //scale.get_units() returns a float
  //Serial.print(" kg "); //You can change this to kg but you'll need to refactor the calibration_factor

  //Serial.print(" Sensor 2: ");
  Serial.println(scale_sensor2.get_units(), 1); //scale.get_units() returns a float
  //Serial.print(" kg "); //You can change this to kg but you'll need to refactor the calibration_factor

  //Serial.print(" Sensor 3: ");
  Serial.println(scale_sensor3.get_units(), 1); //scale.get_units() returns a float
  //Serial.print(" kg "); //You can change this to kg but you'll need to refactor the calibration_factor

  //Serial.print(" Sensor 4: ");
  Serial.println(scale_sensor4.get_units(), 1); //scale.get_units() returns a float
  //Serial.print(" kg "); //You can change this to kg but you'll need to refactor the calibration_factor
  Serial.println("END");
  
}
