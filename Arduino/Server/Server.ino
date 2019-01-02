/**
 *  Created by Brandon Wade
 */
#include <WiFiClient.h>
#include "ESP8266WiFi.h"
#include "ESP8266WebServer.h"

/** Network Global Variables **/
const char* ssid = "OnPointWifi"; // Network Name
const char* password = "Hubbcity2001"; // Password for Network

String readString = String(100); //string for fetching data from address

WiFiServer server(80); // Port to run the server on

String command = String(100); // string to hold the command recieved from client
String lightData = String(100); // holds data to used for LED lights

void setup() {

  Serial.begin(115200);
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }

  PrintWifiStatus();
  
  server.begin(); //Start the server
  Serial.println("Server listening");
 
  delay(10);

}

void loop() {
 ServerListen();
}

/**
 *    Waits for a client to connect, then interpets the information sent by the client and sends the client back a reply if needed.
 */
void ServerListen()
{

  WiFiClient client = server.available(); // listen for incoming clients

  if (client)  // if a client has connected
  {
    Serial.println("client connected");
    
    while (client.connected()) // while the client is connected
    {
      if (client.available()) { // if the client is able to recieve data
        
        char c = client.read(); //Grab first character of the first recieved reply

        if (readString.length() < 100)
        {
          readString.concat(c);
        }


        // Every client will send a command and a message that ends with the '!' character
        // <GRID_INFORMATION_REPLY> +01011-!
        // <LIGHT_INFORMATION_REQUEST> *empty message
        if (c == '!')
        {
          // Handle incoming GRID_INFORMATION command
          if(ParseCommands() == 1)
          {
             PrintGridInformation();
          }
           // Handle incoming LIGHT_INFOMRATION_REQUEST
          if(ParseCommands() == 2)
          {
            PrintGridInformation();
            client.print("Light Data: ");
            client.println(lightData);
            lightData = "";
          }
         
          delay(1);
          //reset strings
          readString = "";
          command = "";
          

        }
      }
    }
  }
}

/**
 *  Returns a int corresponding to the recieved command 
 *  
 *  1 - if client sends GRID_INFORMATION
 *  2 - if client sends LIGHT_INFORMATION
 */
int ParseCommands()
{
   int firstBracket = readString.indexOf('<') + 1;
   int secondBracket = readString.indexOf('>');
   command = readString.substring(firstBracket, secondBracket);

   // Unity sent the arduino information on its current grid in the form of:
   // Example - <GRID_INFORMATION> +01011-
   // where the 0 and 1's represent alive and dead cells in the crid.
   // the '+' or '-' are use as start and end indicators for the commands Message
   if(command.equals("GRID_INFORMATION"))
   {
      int startIndex = readString.indexOf('+') + 1;// start reading from the character after '+'
      int endIndex = readString.indexOf('-'); // stop reading at the character  '-'
      lightData =  readString.substring(startIndex, endIndex); // store the grid information
      return 1;
   }

   // Client Arduino requested the information sent by unity
   // Brodcast a message
   else if(command.equals("LIGHT_INFORMATION_REQUEST"))
   {
       return 2;
   }
   
   return 0;
}

/**
 *  Print information recieved from Unity regarding its Grid
 */
void PrintGridInformation()
{
   Serial.print("Command Recieved: ");
   Serial.println(command);
          
   Serial.print("Grid Information: ");
   Serial.println(lightData);
}

/**
 *  Print information on the Network the Arduino Server is connected too
 */
void PrintWifiStatus() {
  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);

  // print the received signal strength:
  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.print(rssi);
  Serial.println(" dBm");
}
