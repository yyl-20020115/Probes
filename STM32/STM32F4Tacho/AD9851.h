#ifndef __AD9851_H__
#define __AD9851_H__

#define AD9851_RCC              RCC_AHB1Periph_GPIOE
#define AD9851_PORT             GPIOE

#define AD9851_PIN_BDATA        GPIO_Pin_0
#define AD9851_PIN_W_CLK        GPIO_Pin_1
#define AD9851_PIN_FQ_UP        GPIO_Pin_2
#define AD9851_PIN_RESET        GPIO_Pin_3

#define AD9851_W0_DEFAULT       0
#define AD9851_W0_ENABLE_6X     1

void AD9851_Init(void);
//0-70MHz Sine Wave and Square Wave
void AD9851_WriteData(unsigned char w0,double frequency);

#endif
