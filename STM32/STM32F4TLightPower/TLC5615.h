#ifndef __TLC5615_H__
#define __TLC5615_H__


#define TLC5615_GPIO_RCC_CS         RCC_AHB1Periph_GPIOF
#define TLC5615_GPIO_PORT_CS        GPIOF
#define TLC5615_GPIO_PIN_CS         GPIO_Pin_13

#define TLC5615_GPIO_RCC_SCLK       RCC_AHB1Periph_GPIOF
#define TLC5615_GPIO_PORT_SCLK      GPIOF 
#define TLC5615_GPIO_PIN_SCLK       GPIO_Pin_12    

#define TLC5615_GPIO_RCC_DIN        RCC_AHB1Periph_GPIOF
#define TLC5615_GPIO_PORT_DIN       GPIOF
#define TLC5615_GPIO_PIN_DIN        GPIO_Pin_11    

void TLC5615_Init(void);

//10 bit DA: 1023(0x3ff)=4.999V (@5.0V)
void TLC5615_WriteDAC(unsigned short DAValue);

#endif
