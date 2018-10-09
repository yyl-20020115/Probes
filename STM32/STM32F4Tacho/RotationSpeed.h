#ifndef __ROTATIONSPEED_H__
#define __ROTATIONSPEED_H__


//TIM2-CH1: PIN=PA0
#define ROTATIONSPEED_TIM           TIM2
#define ROTATIONSPEED_TIM_IT_CC     (TIM_IT_CC1 | TIM_IT_Update)
#define ROTATIONSPEED_TIM_RCC       RCC_APB1Periph_TIM2
#define ROTATIONSPEED_TIM_PORT_RCC  RCC_AHB1Periph_GPIOA
#define ROTATIONSPEED_PORT          GPIOA
#define ROTATIONSPEED_PIN           GPIO_Pin_0
#define ROTATIONSPEED_CHANNEL       TIM_Channel_1
#define ROTATIONSPEED_TIM_IRQ       TIM2_IRQn
#define ROTATIONSPEED_PIN_SOURCE		GPIO_PinSource0
#define ROTATIONSPEED_TIM_AF				GPIO_AF_TIM2
#define ROTATIONSPEED_CAPTURE()			TIM_GetCapture1(ROTATIONSPEED_TIM)
#define ROTATIONSPEED_FLAG					TIM_FLAG_CC1


#define ROTATIONSPEED_IRQ_HANDLER		TIM2_IRQHandler

#define ROTATIONSPEED_ENCODER_HOLES 1

//120K RPM: PRESCALER = SystemCoreClock /(ROTATIONSPEED_MAX_RPM /60.0 * 20) = 168000000 /(120000 /60 * 20) =  4200
#define ROTATIONSPEED_MAX_RPM       240000
//max value of uint32
#define ROTATIONSPEED_PERIOD        0x100000000

#define ROTATIONSPEED_FILTER        0x0f

#define ROTATIONSPEED_1M						1000000

#define PERIOD_BUFSIZE 10

void RS_Init(void);
unsigned int RS_GetSpeed(void);

void RS_ClearSpeed(void);

float RS_GetSpeed_RPS(void);

float RS_GetSpeed_RPM(void);

void RotationSpeedIRQ(void);

#endif
