/*
  Kill Switch Monitor
  Authors: 
  William Johnson
  Angela Hudak
  
*/

int aliveOutputPin = 37; 
int aliveInputPin = 39;
int killOutputPin = 35;
int killInputPin = 41;
int kill = 0;      // variable to store the read value
int alive = 0;      // variable to store the read value
int readingAlive = 1;
void setup(){
  Serial.begin(115200);
  
  pinMode(killOutputPin, OUTPUT);   // sets the digital pin as output
  pinMode(killInputPin, INPUT);     // sets the digital pin as input
  pinMode(aliveOutputPin, OUTPUT);  // sets the digital pin as output
  pinMode(aliveInputPin, INPUT);    // sets the digital pin as input
  digitalWrite(aliveOutputPin, LOW);
  digitalWrite(killOutputPin, LOW);
  readingAlive = 1;
}

void loop(){
//Alive: 1 Kill: 2
  if(readingAlive == 1){
    alive = 0;
    digitalWrite(aliveOutputPin, HIGH);   // send signal through alive output pin
    delay(1);
    alive = digitalRead(aliveInputPin);   // reads the alive input pin
    if(alive == 1){
      Serial.print("alive 2 ");
    } else{
      Serial.print("alive 1 ");
      readingAlive = 0;
    }
    digitalWrite(aliveOutputPin, LOW);
  } else{
    kill = 0;
    digitalWrite(killOutputPin, HIGH);   // send signal through kill output pin
    delay(1);
    kill = digitalRead(killInputPin);   // reads the kill input pin
    if(kill == 1){
      Serial.print("kill 1 ");
    } else{
      Serial.print("kill 2 ");
      readingAlive = 1;
    }
    digitalWrite(killOutputPin, LOW);
  }
  delay(500);
}
