
//////////
/////////  All Serial Handling Code,
/////////  It's Changeable with the 'outputType' variable
/////////  It's declared at start of code.
/////////
#define BPM_BASE 100
#define IBI_BASE 1000
#define SIGNAL_BASE 1000
void serialOutput(bool QS){   // Decide How To Output Serial.
// open the Arduino Serial Plotter to visualize these data
      Serial.print("HEART:");
      Serial.print(QS?"B":"N");
      Serial.print(",");
      Serial.print(BPM + BPM_BASE);
      Serial.print(",");
      Serial.print(IBI + IBI_BASE);
      Serial.print(",");
      Serial.println(Signal + SIGNAL_BASE);
}
