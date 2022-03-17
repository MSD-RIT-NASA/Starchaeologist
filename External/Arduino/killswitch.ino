/*
  Kill Switch Monitor
*/

int aliveOutputPin = 43; 
int aliveInputPin = 35;
int killOutputPin = 51;
int killInputPin = 29;
int kill = 0;      // variable to store the read value
int alive = 0;      // variable to store the read value
int readingAlive = 1;
void setup(){
  Serial.begin(115200);
  pinMode(killOutputPin, OUTPUT);   // sets the digital pin as output
  pinMode(killInputPin, INPUT);     // sets the digital pin as input
  pinMode(aliveOutputPin, OUTPUT);  // sets the digital pin as output
  pinMode(aliveInputPin, INPUT);    // sets the digital pin as input
}

void loop(){
  if(readingAlive == 1){
    alive = 0;
    digitalWrite(aliveOutputPin, HIGH);   // sets the LED on
    alive = digitalRead(aliveInputPin);   // read the input pin
    if(alive == 1){
      Serial.write("1");
    } else{
      Serial.write("2");
      readingAlive = 0;
    }
  } else{
    kill = 0;
    digitalWrite(killOutputPin, HIGH);   // sets the LED on
    kill = digitalRead(killInputPin);   // read the input pin
    if(kill == 1){
      Serial.write("2");
    } else{
      Serial.write("1");
      readingAlive = 1;
    }
  }
  delay(500);
}