#ifndef __DAC8560_H__
#define __DAC8560_H__

#define DAC8560_RCC                 RCC_AHB1Periph_GPIOB

#define DAC8560_GPIO_PORT_SYNC      GPIOB
#define DAC8560_GPIO_PIN_SYNC       GPIO_Pin_0
#define DAC8560_GPIO_PORT_CLK       GPIOB 
#define DAC8560_GPIO_PIN_CLK        GPIO_Pin_1   
#define DAC8560_GPIO_PORT_DIN       GPIOB
#define DAC8560_GPIO_PIN_DIN        GPIO_Pin_2   

#define PD_OPERATION_NORMAL         0
#define PD_OPERATION_1KTOGND        1
#define PD_OPERATION_100KToGND      2
#define PD_OPERATION_HIGHZ          3

void DAC8560_Init(void);
//16 bit DA:2.5V=0xffff
void DAC8560_WriteDAC(unsigned short DAValue, unsigned char op);

#endif

