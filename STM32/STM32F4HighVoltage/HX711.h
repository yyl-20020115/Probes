#ifndef __HX711_H__
#define __HX711_H__

#define HX711_RCC       RCC_AHB1Periph_GPIOD
#define HX711_PORT      GPIOD
#define HX711_SCK_PIN   GPIO_Pin_0													  
#define HX711_DOUT_PIN  GPIO_Pin_1

void HX711_Init(void);

//24BIT ADC SCALE
int HX711_Read(void);

void HX711_Calibrate(void);

int HX711_GetMiddleFilterValue(int rc);

extern int _ZeroLevel;
extern int _100gLevel;
extern int _500gLevel;

extern int RealZeroLevel;
extern int Real100gLevel;
extern int Real500gLevel;

#endif
