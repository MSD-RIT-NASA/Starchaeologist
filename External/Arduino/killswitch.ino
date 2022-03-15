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
  Serial.begin(9600);
  pinMode(killOutputPin, OUTPUT);   // sets the digital pin as output
  pinMode(killInputPin, INPUT);     // sets the digital pin as input
  pinMode(aliveOutputPin, OUTPUT);  // sets the digital pin as output
  pinMode(aliveInputPin, INPUT);    // sets the digital pin as input
  Serial.setTimeout(1);

}

void loop(){
  if(readingAlive == 1){
    alive = 0;
    digitalWrite(aliveOutputPin, HIGH);   // sets the LED on
//    delay(100);                  
    alive = digitalRead(aliveInputPin);   // read the input pin
    if(alive == 1){
      Serial.print("alive");
    } else{
      readingAlive = 0;
//      Serial.println("Now Dead");
    }
  } else{
    kill = 0;
    digitalWrite(killOutputPin, HIGH);   // sets the LED on
//    delay(100);                  // waits for a second
    kill = digitalRead(killInputPin);   // read the input pin
    if(kill == 1){
      Serial.print("kill");
    } else{
      readingAlive = 1;
//      Serial.println("Now Alive");
    }
  }
}