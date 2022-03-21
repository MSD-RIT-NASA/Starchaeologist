/*
  Kill Switch Monitor
*/

int aliveOutputPin = 7; 
int aliveInputPin = 8;
int killOutputPin = 5;
int killInputPin = 10;
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
//Alive: 1 Kill: 2
  if(readingAlive == 1){
    alive = 0;
    digitalWrite(aliveOutputPin, HIGH);   // sets the LED on
    alive = digitalRead(aliveInputPin);   // read the input pin
    if(alive == 1){
      Serial.print("2");
    } else{
      Serial.print("1");
      readingAlive = 0;
    }
  } else{
    kill = 0;
    digitalWrite(killOutputPin, HIGH);   // sets the LED on
    kill = digitalRead(killInputPin);   // read the input pin
    if(kill == 1){
      Serial.print("1");
    } else{
      Serial.print("2");
      readingAlive = 1;
    }
  }
  delay(500);
}