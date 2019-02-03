#include "usart.h"
#include <stdio.h>
//PIN:PB6,PB7
//��ʼ������1
void COM1Init(u32 BaudRate)
{
  	GPIO_InitTypeDef GPIO_InitStructure;
  	USART_InitTypeDef USART_InitStructure;
  	NVIC_InitTypeDef	 NVIC_InitStructure;

  	RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOB, ENABLE);//����ʱ��ʹ�� 
  	RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1, ENABLE);
  	GPIO_PinAFConfig(GPIOB, GPIO_PinSource6, GPIO_AF_USART1);//���Ӹ�������  
  	GPIO_PinAFConfig(GPIOB, GPIO_PinSource7, GPIO_AF_USART1);
  	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_6 | GPIO_Pin_7;
  	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
  	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
  	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;
  	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	
		NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1); 
		NVIC_InitStructure.NVIC_IRQChannel = USART1_IRQn;
		NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0; 
		NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0; 
		NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE; 
		NVIC_Init(&NVIC_InitStructure);

	
	
  	GPIO_Init(GPIOB, &GPIO_InitStructure);//��ʼ������2��GPIO   
  	USART_InitStructure.USART_BaudRate = BaudRate;//����������
  	USART_InitStructure.USART_WordLength = USART_WordLength_8b;//8λ����ģʽ
  	USART_InitStructure.USART_StopBits = USART_StopBits_1;//1λֹͣλ
  	USART_InitStructure.USART_Parity = USART_Parity_No;//����żУ��λ
  	USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;//��Ӳ���������
  	USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;//˫��ģʽ 
  	USART_Init(USART1, &USART_InitStructure);
  	USART_Cmd(USART1, ENABLE);
  	USART_ClearFlag(USART1, USART_FLAG_TC);//�崫����ɱ�־
}

//����print֧��
#if 1
#pragma import(__use_no_semihosting)                              
struct __FILE 
{ 
	int handle; 
}; 
FILE __stdout;         
int _sys_exit(int x) 
{ 
	return (x = x); 
} 
int fputc(int ch, FILE *f)
{
  	USART_SendData(USART1, (u8) ch);//����1����һ���ַ�
  	while (USART_GetFlagStatus(USART1, USART_FLAG_TC) == RESET);//�ȴ��������
  	return ch;
}
#endif
