//V1.0.0
#include "stm32f4xx_it.h"
#include "main.h"

//NMI exception handler
void NMI_Handler(void)
{
}

//Hard Fault exception handler
void HardFault_Handler(void)
{
	LCD_String(20,10,(const u8*)"HardFault",RED);
}

//Memory Manage exception handler
void MemManage_Handler(void)
{
  	while (1)
  	{
  	}
}

//Bus Fault exception handler
void BusFault_Handler(void)
{
  	while (1)
  	{
  	}
}

//Usage Fault exception handler
void UsageFault_Handler(void)
{
  	while (1)
  	{
  	}
}

//SVCall exception handler
void SVC_Handler(void)
{
}

//Debug Monitor exception handler
void DebugMon_Handler(void)
{
}

//PendSVC exception handler
void PendSV_Handler(void)
{
}

//SysTick handler
extern u32 ntime;
extern u32 LocalTime;
void SysTick_Handler(void)
{
	if(ntime>0)ntime--;
	LocalTime+=10;
}
void USART2_IRQHandler()
{
    if(USART_GetITStatus(USART2, USART_IT_RXNE) != RESET)	   //判断读寄存器是否非空
    {	
        uint16_t c = USART_ReceiveData(USART2);   //将读寄存器的数据缓存到接收缓冲区里
		

    }
  
    if(USART_GetITStatus(USART2, USART_IT_TXE) != RESET)                   //这段是为了避免STM32 USART 第一个字节发不出去的BUG 
    { 
        USART_ITConfig(USART2, USART_IT_TXE, DISABLE);					     //禁止发缓冲器空中断， 
    }	
}

