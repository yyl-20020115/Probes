#ifndef __FREQUENCYMETER_H__
#define __FREQUENCYMETER_H__


//TIM2-CH1: PIN=PA0
#define FREQUENCYMETER_TIM           TIM2
#define FREQUENCYMETER_TIM_IT_CC     (TIM_IT_CC1 | TIM_IT_Update)
#define FREQUENCYMETER_TIM_RCC       RCC_APB1Periph_TIM2
#define FREQUENCYMETER_TIM_PORT_RCC  RCC_AHB1Periph_GPIOA
#define FREQUENCYMETER_PORT          GPIOA
#define FREQUENCYMETER_PIN           GPIO_Pin_0
#define FREQUENCYMETER_CHANNEL       TIM_Channel_1
#define FREQUENCYMETER_TIM_IRQ       TIM2_IRQn
#define FREQUENCYMETER_PIN_SOURCE		 GPIO_PinSource0
#define FREQUENCYMETER_TIM_AF			   GPIO_AF_TIM2
#define FREQUENCYMETER_CAPTURE()	   TIM_GetCapture1(FREQUENCYMETER_TIM)
#define FREQUENCYMETER_FLAG					 TIM_FLAG_CC1


#define FREQUENCYMETER_IRQ_HANDLER		TIM2_IRQHandler


//max value of uint32
#define FREQUENCYMETER_PERIOD        0x100000000

#define FREQUENCYMETER_FILTER        0

#define FREQUENCYMETER_SYS_FREQUENCY 168000000

#define PERIOD_BUFSIZE 10

void FM_Init(void);

void FM_ClearPeriod(void);

uint32_t FM_GetPeriod(void);
uint32_t FM_GetSysFrequency(void);

float FM_GetFrequency(void);

void FM_IRQ(void);

#endif
