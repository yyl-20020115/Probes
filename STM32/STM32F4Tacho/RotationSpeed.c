#include "stm32f4_discovery.h"
#include "stm32f4xx_tim.h"
#include "stm32f4xx_gpio.h"
#include "RotationSpeed.h"
#include <string.h>

__IO uint32_t Period = 0;
__IO uint32_t LastCapture = 0;
__IO uint32_t ThisCapture = 0;
__IO uint32_t CaptureStarted  = 0;


TIM_TimeBaseInitTypeDef  TIM_TimeBaseStructure = {0};

void RS_Init(void)
{
	GPIO_InitTypeDef RSGPIO_InitStructure = {0};
	NVIC_InitTypeDef RSNVIC_InitStructure = {0};
	TIM_ICInitTypeDef TIM_ICInitStructure = {0};

	
	RCC_APB1PeriphClockCmd(ROTATIONSPEED_TIM_RCC, ENABLE);
	RCC_AHB1PeriphClockCmd(ROTATIONSPEED_TIM_PORT_RCC, ENABLE);

	
	RSGPIO_InitStructure.GPIO_Pin =  ROTATIONSPEED_PIN;
	RSGPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	RSGPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	RSGPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;
	RSGPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
	GPIO_Init(ROTATIONSPEED_PORT, &RSGPIO_InitStructure);
	
	GPIO_PinAFConfig(ROTATIONSPEED_PORT, ROTATIONSPEED_PIN_SOURCE, ROTATIONSPEED_TIM_AF); 


	TIM_TimeBaseStructure.TIM_Prescaler = (SystemCoreClock / ROTATIONSPEED_SYS_FREQUENCY) -1;

	TIM_TimeBaseStructure.TIM_Period = ROTATIONSPEED_PERIOD - 1;	
	TIM_TimeBaseStructure.TIM_ClockDivision = TIM_CKD_DIV1;
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
	TIM_TimeBaseStructure.TIM_RepetitionCounter = 0;
	TIM_TimeBaseInit(ROTATIONSPEED_TIM, &TIM_TimeBaseStructure);

   
	TIM_ICInitStructure.TIM_Channel = ROTATIONSPEED_CHANNEL;
	TIM_ICInitStructure.TIM_ICPolarity = TIM_ICPolarity_Rising;
	TIM_ICInitStructure.TIM_ICSelection = TIM_ICSelection_DirectTI;
	
	//NOTICE: This has to be DIV2;otherwise, result is doubled
	TIM_ICInitStructure.TIM_ICPrescaler = TIM_ICPSC_DIV2;
	TIM_ICInitStructure.TIM_ICFilter = ROTATIONSPEED_FILTER;

	TIM_ICInit(ROTATIONSPEED_TIM, &TIM_ICInitStructure);

	RSNVIC_InitStructure.NVIC_IRQChannel = ROTATIONSPEED_TIM_IRQ;
	RSNVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	RSNVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
	RSNVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&RSNVIC_InitStructure);

	TIM_ITConfig(ROTATIONSPEED_TIM, TIM_IT_Update|ROTATIONSPEED_TIM_IT_CC, ENABLE);	

	TIM_Cmd(ROTATIONSPEED_TIM, ENABLE);
	
}
void RS_ClearPeriod(void)
{
    Period = 0;  
}

unsigned int RS_GetPeriod(void)
{
    return Period;
}

unsigned int RS_GetSysFrequency(void)
{
    return SystemCoreClock;
}

void RotationSpeedIRQ(void)
{
	if(TIM_GetITStatus(ROTATIONSPEED_TIM, ROTATIONSPEED_TIM_IT_CC) == SET) 
	{			
	
			if(CaptureStarted == 0)
			{
					LastCapture = ROTATIONSPEED_CAPTURE();
					CaptureStarted=1;
			}
			else
			{
					ThisCapture = ROTATIONSPEED_CAPTURE();
					Period = (ThisCapture - LastCapture); 
					LastCapture = ThisCapture;
			}
			TIM_ClearITPendingBit(ROTATIONSPEED_TIM, ROTATIONSPEED_TIM_IT_CC);
	}
	else if (TIM_GetITStatus(ROTATIONSPEED_TIM, TIM_IT_Update) == SET)
	{
		ThisCapture = ROTATIONSPEED_CAPTURE();
		Period = ((ROTATIONSPEED_PERIOD - LastCapture) + ThisCapture); 								
		LastCapture = ThisCapture;
		TIM_ClearITPendingBit(ROTATIONSPEED_TIM, TIM_IT_Update);
	}
}
