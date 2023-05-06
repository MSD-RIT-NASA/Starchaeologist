#include <Arduino.h>
#include <Adafruit_BNO08x.h>
#include <ESP8266WiFi.h>
#include <WiFiUdp.h>
#include <Ticker.h>

// Wifi-related declarations
#define _WIFI_SSID "MSD_ISS"
#define _WIFI_PSK "horizons"
#define DEBUG 1
//#define BNO08X_INT 14
#define BNO08X_RESET -1
#define COMM_PORT 8888
#define BRIGHTNESS 32

WiFiServer server(COMM_PORT);
WiFiEventHandler wifiConnectHandler;
WiFiEventHandler wifiDisconnectHandler;
WiFiUDP Udp;
unsigned int localUdpPort = 4210;  // local port to listen on
char incomingPacket[255];  // buffer for incoming packets
char replyPacket[255]; // buffer for outgoing packets
IPAddress hostIP;
int hostPort;

char outMessage[1024];

Adafruit_BNO08x  bno08x(BNO08X_RESET);
sh2_SensorValue_t sensorValue;
WiFiClient wificlient;

char real_value[64];
char i_value[64];
char j_value[64];
char k_value[64];
 
const float _reconnect_time_sec = 10;
Ticker wifiReconnectTimer;
 
// WiFi functions
void wifi_init();
void wifi_connect();
void wifi_afterConnect();
void onWifiConnected(const WiFiEventStationModeGotIP& event);
void onWifiDisconnected(const WiFiEventStationModeDisconnected& event);
volatile bool _wifi_connected = false;

//////////////////////////////////////////////////////
//                      LED functions               //
//////////////////////////////////////////////////////

void initLED(void) {
    pinMode(12, OUTPUT);
    pinMode(13, OUTPUT);
    pinMode(15, OUTPUT);
}

void redLED(void) {
  analogWrite(12, BRIGHTNESS);
  analogWrite(13, 0);
  analogWrite(15, 0);
}

void greenLED(void) {
  analogWrite(12, 0);
  analogWrite(13, BRIGHTNESS);
  analogWrite(15, 0);
}

void blueLED(void) {
  analogWrite(12, 0);
  analogWrite(13, 0);
  analogWrite(15, BRIGHTNESS);
}

void magentaLED(void) {
  analogWrite(12, BRIGHTNESS);
  analogWrite(13, 0);
  analogWrite(15, BRIGHTNESS);
}

void yellowLED(void) {
  analogWrite(12, BRIGHTNESS);
  analogWrite(13, BRIGHTNESS);
  analogWrite(15, 0);
}

void cyanLED(void) {
  analogWrite(12, 0);
  analogWrite(13, BRIGHTNESS);
  analogWrite(15, BRIGHTNESS);
}

void whiteLED(void) {
  analogWrite(12, BRIGHTNESS);
  analogWrite(13, BRIGHTNESS);
  analogWrite(15, BRIGHTNESS);
}


//////////////////////////////////////////////////////
//                      WiFi functions              //
//////////////////////////////////////////////////////
 
/** Some initial WiFi setup */
void wifi_init() {
    WiFi.disconnect();
    delay(1200);
    wifiConnectHandler = WiFi.onStationModeGotIP(onWifiConnected);
}
 
/**
 * This function connects to WiFi and sets a timer in case connection is not
 * established during a defined interval of _reconnect_time_sec.
 */
void wifi_connect(void) {
    #ifdef DEBUG
    Serial.println("Connecting to Wi-Fi...");
    #endif
    WiFi.mode(WIFI_STA);
    WiFi.begin(_WIFI_SSID, _WIFI_PSK);
    // Start timer to retry if nothing happens in the configured time
    wifiReconnectTimer.once(_reconnect_time_sec, wifi_connect);
}
 
/** Callback when WiFi is connected */
void onWifiConnected(const WiFiEventStationModeGotIP& event) {
    #ifdef DEBUG
    Serial.println("Connected to Wi-Fi.");
    Serial.printf("IP: %s\nMAC Address: ", WiFi.localIP().toString().c_str());
    Serial.println(WiFi.macAddress());
    #endif

    wifiDisconnectHandler = WiFi.onStationModeDisconnected(onWifiDisconnected);
    _wifi_connected = true;
 
    // Cancel running wifiReconnectTimer
    wifiReconnectTimer.detach();
    wifi_afterConnect();
}
 
/** Callback when WiFi is disconnected */
void onWifiDisconnected(const WiFiEventStationModeDisconnected& event) {
    #ifdef DEBUG
    Serial.println("Wi-Fi disconnected. Trying to reconnect!");
    #endif
 
    _wifi_connected = false;
    wifi_connect();
}
 
void wifi_afterConnect() {
  //
}

void setReports(void) {
  Serial.println("Setting desired reports");
  if (! bno08x.enableReport(SH2_GAME_ROTATION_VECTOR)) {
    Serial.println("Could not enable game vector");
    redLED();
  }
}

void sensor_init() {  
  if (!bno08x.begin_I2C()) {
    Serial.println("Failed to find BNO08x chip");
    redLED();
    while (!bno08x.begin_I2C()) { bno08x.hardwareReset(); delay(1000);  }
  }
  Serial.println("BNO08x Found!");
  if (!bno08x.enableReport(SH2_GAME_ROTATION_VECTOR)) {
    Serial.println("Could not enable game vector");
    redLED();
  }
  Serial.println("Sensor initialized");
}

void setup() {
  whiteLED();
  #ifdef DEBUG
  Serial.begin(115200);
  #endif
  delay(100);
  wifi_init();
  blueLED();
  delay(1000);
  wifi_connect();
  while(!_wifi_connected) {
    delay(500);
  }
  yellowLED();
  Udp.begin(localUdpPort);
  Serial.printf("Now listening on UDP port %d\n", localUdpPort);

  int packetSize = 0;
  while(!packetSize) {
    packetSize = Udp.parsePacket();
    delay(500);
  }
  greenLED();
  Serial.printf("Received %d bytes from %s, port %d\n", packetSize, Udp.remoteIP().toString().c_str(), Udp.remotePort());
  hostIP = Udp.remoteIP();
  hostPort = Udp.remotePort();
  delay(1000);
  sensor_init();
  delay(100);
  bno08x.setOrientation(-1.0, 0.0, 0.0, 0.0);
}
 
void loop() {

    // If we recieve another packet, parse it and make that node the new reciever
    int packetSize = Udp.parsePacket();
    if (packetSize) {
      hostIP = Udp.remoteIP();
      hostPort = Udp.remotePort();
      int len = Udp.read(incomingPacket, 255);
      if (len > 0)
      {
        // Parse packet, see if it is a control packet
        char cmd_reset[] = "reset";
        if(strcmp(incomingPacket, cmd_reset) == 0) {
          magentaLED();
          bno08x.hardwareReset();
          delay(1000);
          //ESP.restart();
        }
      }
    }
 
    if (bno08x.wasReset()) {
      Udp.beginPacket(hostIP, hostPort);
      strcpy(replyPacket, "Sensor was reset");
      Udp.write(replyPacket);
      Udp.endPacket();
      setReports();
    }
  
    if (!bno08x.getSensorEvent(&sensorValue)) {
      return;
    }

    greenLED();
    switch (sensorValue.sensorId) {
      case SH2_GAME_ROTATION_VECTOR:
        Udp.beginPacket(hostIP, hostPort);
        dtostrf(sensorValue.un.gameRotationVector.i, 4, 3, i_value);
        strcpy(replyPacket, i_value);
        Udp.write(replyPacket);
        Udp.endPacket();
        break;
    }
    delay(25);
}
