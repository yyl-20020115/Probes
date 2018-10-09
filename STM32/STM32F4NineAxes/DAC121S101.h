#ifndef __DAC121S101_H__
#define __DAC121S101_H__

#define DAC121S101_GPIO_RCC            RCC_AHB1Periph_GPIOD

#define DAC121S101_GPIO_PORT_SYNC      GPIOD
#define DAC121S101_GPIO_PIN_SYNC       GPIO_Pin_13
#define DAC121S101_GPIO_PORT_CLK       GPIOD 
#define DAC121S101_GPIO_PIN_CLK        GPIO_Pin_14   
#define DAC121S101_GPIO_PORT_DIN       GPIOD
#define DAC121S101_GPIO_PIN_DIN        GPIO_Pin_15   

#define PD_OPERATION_NORMAL         0
#define PD_OPERATION_1KTOGND        1
#define PD_OPERATION_100KToGND      2
#define PD_OPERATION_HIGHZ          3

void DAC121S101_Init(void);
//12 bit DA
void DAC121S101_WriteDAC(unsigned short DAValue, unsigned char op);


#endif
