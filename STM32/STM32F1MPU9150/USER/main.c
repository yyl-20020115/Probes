#include "stm32f10x.h"
#include "STM32_I2C.h"
#include "DataScope_DP.h"
#include "delay.h"
#include "led.h"
#include "mpu.h"
#include "usart.h"

#define Accel_Zout_Offset		600
#define Gyro_Xout_Offset	    -70
#define Gyro_Yout_Offset		25
#define Gyro_Zout_Offset		-10
#define Accel_Xout_H		    0x3B

#define Gyro_500_Scale_Factor   65.5f
#define Accel_4_Scale_Factor    8192.0f

#define MAIN_USART USART1


#ifdef __GNUC__
  /* With GCC/RAISONANCE, small printf (option LD Linker->Libraries->Small printf
     set to 'Yes') calls __io_putchar() */
  #define PUTCHAR_PROTOTYPE int __io_putchar(int ch)
#else
  #define PUTCHAR_PROTOTYPE int fputc(int ch, FILE *f)
#endif /* __GNUC__ */

PUTCHAR_PROTOTYPE
{
  /* Place your implementation of fputc here */
  /* e.g. write a character to the USART */
  USART_SendData(USART1, (uint8_t) ch);

  /* 循环等待直到发送结束*/
  while (USART_GetFlagStatus(USART1, USART_FLAG_TC) == RESET)
  {}

  return ch;
}


unsigned short mag[3] = {0};

unsigned long timestamp = 0;

void Init_compass(void) ;
void init_quaternion(void);
void Delay(__IO uint32_t nCount);
void GPIO_Configuration(void);

int main(void)
{
	 unsigned short Ax = 0,Ay = 0,Az= 0,Gx = 0, Gy = 0, Gz = 0;
	 SystemInit(); 
	 uart_init(115200);

	 GPIO_Configuration();
	 Init_MPU9150();
	 mpu_init();
	  //mpu_set_sensor，使用电子罗盘，要加入INV_XYZ_COMPASS
	 mpu_set_sensors(INV_XYZ_GYRO | INV_XYZ_ACCEL | INV_XYZ_COMPASS);
	 while(1)
   {
		mpu_get_compass_reg(mag, &timestamp);  //读取compass数据
		
		Gx=GetData(GYRO_XOUT_H);
		Gy=GetData(GYRO_YOUT_H);
		Gz=GetData(GYRO_ZOUT_H);
		Ax=GetData(ACCEL_XOUT_H);
		Ay=GetData(ACCEL_YOUT_H);
		Az=GetData(ACCEL_ZOUT_H);
    printf("NA:%04X,%04X,%04X,%04X,%04X,%04X,%04X,%04X,%04X\n",Ax,Ay,Az,Gx,Gy,Gz,mag[0],mag[1],mag[2]);
    delay_ms(10); 
	} 
}
 
void GPIO_Configuration(void)
{
  GPIO_InitTypeDef  GPIO_InitStructure;
  //RCC_APB2PeriphClockCmd(RCC_APB1Periph_I2C1, ENABLE); 	
  RCC_APB2PeriphClockCmd( RCC_APB2Periph_GPIOB , ENABLE  );
  GPIO_InitStructure.GPIO_Pin =  GPIO_Pin_6;
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_OD;  
  GPIO_Init(GPIOB, &GPIO_InitStructure);

  GPIO_InitStructure.GPIO_Pin =  GPIO_Pin_7;
  GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
  GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_OD;
  GPIO_Init(GPIOB, &GPIO_InitStructure);
	
}

void Delay(__IO uint32_t nCount)
{
  for(; nCount != 0; nCount--);
}
