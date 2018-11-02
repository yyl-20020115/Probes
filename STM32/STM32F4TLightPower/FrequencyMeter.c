#include "stm32f4_discovery.h"
#include "stm32f4xx_tim.h"
#include "stm32f4xx_gpio.h"
#include "FrequencyMeter.h"
#include <string.h>

__IO uint32_t Period = 0;
__IO uint32_t LastCapture = 0;
__IO uint32_t ThisCapture = 0;
__IO uint32_t CaptureStarted  = 0;


TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure = {0};

void FM_Init(void)
{
	GPIO_InitTypeDef FMGPIO_InitStructure = {0};
	NVIC_InitTypeDef FMNVIC_InitStructure = {0};
	TIM_ICInitTypeDef TIM_ICInitStructure = {0};

	
	RCC_APB1PeriphClockCmd(FREQUENCYMETER_TIM_RCC, ENABLE);
	RCC_AHB1PeriphClockCmd(FREQUENCYMETER_TIM_PORT_RCC, ENABLE);

	
	FMGPIO_InitStructure.GPIO_Pin =  FREQUENCYMETER_PIN;
	FMGPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	FMGPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	FMGPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;
	FMGPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz; //STM32F4:50MHz-200MHz
	GPIO_Init(FREQUENCYMETER_PORT, &FMGPIO_InitStructure);
	
	GPIO_PinAFConfig(FREQUENCYMETER_PORT, FREQUENCYMETER_PIN_SOURCE, FREQUENCYMETER_TIM_AF); 

	

	TIM_TimeBaseStructure.TIM_Prescaler = (SystemCoreClock / FREQUENCYMETER_SYS_FREQUENCY) -1;

	TIM_TimeBaseStructure.TIM_Period = FREQUENCYMETER_PERIOD - 1;	
	TIM_TimeBaseStructure.TIM_ClockDivision = TIM_CKD_DIV1;
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
	TIM_TimeBaseStructure.TIM_RepetitionCounter = 0;
	TIM_TimeBaseInit(FREQUENCYMETER_TIM, &TIM_TimeBaseStructure);

   
	TIM_ICInitStructure.TIM_Channel = FREQUENCYMETER_CHANNEL;
	TIM_ICInitStructure.TIM_ICPolarity = TIM_ICPolarity_Rising;
	TIM_ICInitStructure.TIM_ICSelection = TIM_ICSelection_DirectTI;
	
	//NOTICE: This has to be DIV2;otherwise, result is doubled
	TIM_ICInitStructure.TIM_ICPrescaler = TIM_ICPSC_DIV2;
	TIM_ICInitStructure.TIM_ICFilter = FREQUENCYMETER_FILTER;

	TIM_ICInit(FREQUENCYMETER_TIM, &TIM_ICInitStructure);

	FMNVIC_InitStructure.NVIC_IRQChannel = FREQUENCYMETER_TIM_IRQ;
	FMNVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	FMNVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
	FMNVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&FMNVIC_InitStructure);

	TIM_ITConfig(FREQUENCYMETER_TIM, TIM_IT_Update | FREQUENCYMETER_TIM_IT_CC, ENABLE);	

	TIM_Cmd(FREQUENCYMETER_TIM, ENABLE);
	
}
void FM_ClearPeriod(void)
{
    Period = 0;  
}

uint32_t FM_GetPeriod(void)
{
	return Period;
}
uint32_t FM_GetSysFrequency(void)
{
	return SystemCoreClock;
}

float FM_GetFrequency(void)
{
		if(Period!=0)
		{
			return SystemCoreClock /(float)(Period);
		}
		else
		{
			return 0.0f;
		}
}

void FM_IRQ(void)
{
    if(TIM_GetITStatus(FREQUENCYMETER_TIM, FREQUENCYMETER_TIM_IT_CC) == SET) 
    {			
			if(CaptureStarted == 0)
			{
					LastCapture = FREQUENCYMETER_CAPTURE();
					CaptureStarted = 1;
			}
			else
			{
					ThisCapture = FREQUENCYMETER_CAPTURE();
	
					Period = (ThisCapture - LastCapture); 
					LastCapture = ThisCapture;
			}
			
			TIM_ClearITPendingBit(FREQUENCYMETER_TIM, FREQUENCYMETER_TIM_IT_CC);
    }
		else if (TIM_GetITStatus(FREQUENCYMETER_TIM, TIM_IT_Update) == SET)
		{
			ThisCapture = FREQUENCYMETER_CAPTURE();
			Period = ((FREQUENCYMETER_PERIOD - LastCapture) + ThisCapture); 								
			LastCapture = ThisCapture;
			TIM_ClearITPendingBit(FREQUENCYMETER_TIM, TIM_IT_Update);
		}
}
