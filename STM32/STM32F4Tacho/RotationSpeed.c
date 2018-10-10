#include "stm32f4_discovery.h"
#include "stm32f4xx_tim.h"
#include "stm32f4xx_gpio.h"
#include "RotationSpeed.h"
#include <string.h>

__IO uint32_t Period = 0;
__IO uint32_t LastCapture = 0;
__IO uint32_t ThisCapture = 0;
__IO uint32_t CaptureStarted  = 0;


float RS_RPM_TO_RPS(float rpm);
float RS_RPS_TO_TPS(float rps);

unsigned short RS_TPS_TO_PRESCALER(float tps);
	

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

	

	TIM_TimeBaseStructure.TIM_Prescaler = (SystemCoreClock / ROTATIONSPEED_1M) -1;

	TIM_TimeBaseStructure.TIM_Period = ROTATIONSPEED_PERIOD - 1;	
	TIM_TimeBaseStructure.TIM_ClockDivision = 0;
	TIM_TimeBaseStructure.TIM_CounterMode = TIM_CounterMode_Up;
	TIM_TimeBaseStructure.TIM_RepetitionCounter = 0;
	TIM_TimeBaseInit(ROTATIONSPEED_TIM, &TIM_TimeBaseStructure);

   
	TIM_ICInitStructure.TIM_Channel = ROTATIONSPEED_CHANNEL;
	TIM_ICInitStructure.TIM_ICPolarity = TIM_ICPolarity_Falling;
	TIM_ICInitStructure.TIM_ICSelection = TIM_ICSelection_DirectTI;
	
	//NOTICE: here should be TIM_ICPSC_DIV1,
	//but it get double speed of the real value,
	//so we have to divide it by 2.
	TIM_ICInitStructure.TIM_ICPrescaler = TIM_ICPSC_DIV1;
	TIM_ICInitStructure.TIM_ICFilter = ROTATIONSPEED_FILTER;

	TIM_ICInit(ROTATIONSPEED_TIM, &TIM_ICInitStructure);

	RSNVIC_InitStructure.NVIC_IRQChannel = ROTATIONSPEED_TIM_IRQ;
	RSNVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
	RSNVIC_InitStructure.NVIC_IRQChannelSubPriority = 1;
	RSNVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
	NVIC_Init(&RSNVIC_InitStructure);

	TIM_ITConfig(ROTATIONSPEED_TIM, ROTATIONSPEED_TIM_IT_CC, ENABLE);	

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

float RS_GetSpeed_RPS(void)
{
    float interval_per_tick = (float)(TIM_TimeBaseStructure.TIM_Prescaler+1)/(float)SystemCoreClock;
    
		uint32_t _Period = RS_GetPeriod();
	
		if(_Period!=0)
		{
			float duration_per_tick = _Period *interval_per_tick;
			
			float duraiton_per_cycle = duration_per_tick * ROTATIONSPEED_ENCODER_HOLES;
			
			float cycles_per_second = 1.0f / duraiton_per_cycle;
			
			return cycles_per_second;
		}
		else
		{
			return 0.0f;
		}
}

float RS_GetSpeed_RPM(void)
{
    return RS_GetSpeed_RPS() * 60.0f;
}

float RS_RPM_TO_RPS(float rpm)
{
    return rpm /60.0f;
}
float RS_RPS_TO_TPS(float rps)
{
    return rps * ROTATIONSPEED_ENCODER_HOLES;
}
unsigned short RS_TPS_TO_PRESCALER(float tps)
{
    return (unsigned short) ((float)SystemCoreClock / tps);
}


void RotationSpeedIRQ(void)
{

    if(TIM_GetITStatus(ROTATIONSPEED_TIM, ROTATIONSPEED_TIM_IT_CC) != RESET) 
    {			
			  TIM_ClearITPendingBit(ROTATIONSPEED_TIM, ROTATIONSPEED_TIM_IT_CC);
			
					if(CaptureStarted == 0)
					{
							LastCapture = ROTATIONSPEED_CAPTURE();
							CaptureStarted=1;
					}
					else
					{
							ThisCapture = ROTATIONSPEED_CAPTURE();
		
							if(TIM_GetFlagStatus(TIM1,TIM_FLAG_CC1OF) == SET)
							{
								TIM_ClearFlag(ROTATIONSPEED_TIM,TIM_FLAG_CC1OF);
								Period = ((ROTATIONSPEED_PERIOD - LastCapture) + ThisCapture); 								
							}
							else
							{
								Period = (ThisCapture - LastCapture); 
							}
							LastCapture = ThisCapture;

				}
    }
}
