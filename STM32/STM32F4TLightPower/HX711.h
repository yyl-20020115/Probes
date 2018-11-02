#ifndef __HX711_H__
#define __HX711_H__

#define HX711_RCC       RCC_AHB1Periph_GPIOD
#define HX711_PORT      GPIOD
#define HX711_SCK_PIN   GPIO_Pin_0													  
#define HX711_DOUT_PIN  GPIO_Pin_1

void HX711_Init(void);

//24BIT ADC SCALE
int HX711_Read(void);

#endif
