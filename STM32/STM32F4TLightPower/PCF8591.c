#include "stm32f4_discovery.h"
#include "PCF8591.h"

unsigned int PCF8591_ulTimeOut_Time = 0;
unsigned char PCF8591_I2C_Err=0;
unsigned char PCF8591_GetErrorCode(void)
{
    return PCF8591_I2C_Err;
}
void PCF8591_Init(void)
{
    GPIO_InitTypeDef  GPIO_InitStructure;
    I2C_InitTypeDef I2C_InitStructure;
    RCC_ClocksTypeDef   rcc_clocks;
    
    /* GPIO Peripheral clock enable */
    RCC_AHB1PeriphClockCmd(PCF8591_RCC_SCL|PCF8591_RCC_SDA, ENABLE);
    RCC_APB1PeriphClockCmd(PCF8591_RCC, ENABLE);
      /* Reset I2Cx IP */
    RCC_APB1PeriphResetCmd(PCF8591_RCC, ENABLE);
    /* Release reset signal of I2Cx IP */
    RCC_APB1PeriphResetCmd(PCF8591_RCC, DISABLE);

    /* GPIO Peripheral clock enable */

    /*I2C3 configuration*/
    GPIO_PinAFConfig(PCF8591_I2C_SCL_PORT, PCF8591_SCL_SRC_PIN, PCF8591_AF);
    GPIO_PinAFConfig(PCF8591_I2C_SDA_PORT, PCF8591_SDA_SRC_PIN, PCF8591_AF);

    //PA8: I2C3_SCL
    GPIO_InitStructure.GPIO_Pin = PCF8591_I2C_SCL_PIN;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_OD;
    GPIO_InitStructure.GPIO_PuPd  = GPIO_PuPd_NOPULL;
    GPIO_Init(PCF8591_I2C_SCL_PORT, &GPIO_InitStructure);

    //PC9: I2C3_SDA
    GPIO_InitStructure.GPIO_Pin = PCF8591_I2C_SDA_PIN;
    GPIO_Init(PCF8591_I2C_SDA_PORT, &GPIO_InitStructure);

    /* I2C Struct Initialize */
    I2C_DeInit(PCF8591_I2C);
    I2C_InitStructure.I2C_Mode = I2C_Mode_I2C;
    I2C_InitStructure.I2C_DutyCycle = I2C_DutyCycle_2;
    I2C_InitStructure.I2C_OwnAddress1 = 0x00;
    I2C_InitStructure.I2C_Ack = I2C_Ack_Enable;
    I2C_InitStructure.I2C_ClockSpeed = PCF8591_I2C_CLOCK_SPEED;
    I2C_InitStructure.I2C_AcknowledgedAddress = I2C_AcknowledgedAddress_7bit;
    I2C_Init(PCF8591_I2C, &I2C_InitStructure);

    /* I2C Initialize */
    I2C_Cmd(PCF8591_I2C, ENABLE);


    /*超时设置*/
    RCC_GetClocksFreq(&rcc_clocks);
    PCF8591_ulTimeOut_Time = (rcc_clocks.SYSCLK_Frequency /10000); 
}

unsigned char PCF8591_ReadADC(unsigned char Channel)
{
//    
//  unsigned char Val;
//   	IIC_start();               //启动总线
//  	IIC_write(AddWr);             //发送器件地址
//	if(ack==0)
//		return(0);
//   	IIC_write(0x40|Chl);            //发送器件子地址
//	if(ack==0)
//		return(0);
//   	IIC_start();
//   	IIC_write(AddRd);
//    if(ack==0)
//		return(0);
//   	Val=IIC_read();
//   	IIC_ACK(1);                 //发送非应位
//   	IIC_stop();                  //结束总线
//  	return(Val);
    uint8_t readout = 0;
    PCF8591_I2C_Err = 0;
    if(Channel <4)
    {
        uint32_t tmr = 0;

        tmr = PCF8591_ulTimeOut_Time;
        while((--tmr)&&I2C_GetFlagStatus(PCF8591_I2C, I2C_FLAG_BUSY));
        if(tmr==0) PCF8591_I2C_Err++;

        I2C_GenerateSTART(PCF8591_I2C, ENABLE);
        //发送I2C的START信号，接口自动从从设备编程主设备
        tmr = PCF8591_ulTimeOut_Time;
        while((--tmr)&&(!I2C_CheckEvent(PCF8591_I2C, I2C_EVENT_MASTER_MODE_SELECT)));
        if(tmr==0) PCF8591_I2C_Err++;

        I2C_Send7bitAddress(PCF8591_I2C,PCF8591_WRITE_ADDRESS,I2C_Direction_Transmitter);
        tmr = PCF8591_ulTimeOut_Time;
        while((--tmr)&&(!I2C_CheckEvent(PCF8591_I2C,I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED)));
        if(tmr==0) PCF8591_I2C_Err++;

        I2C_SendData(PCF8591_I2C, 0x40|Channel);
        tmr = PCF8591_ulTimeOut_Time;
        while((--tmr)&&(!I2C_CheckEvent(PCF8591_I2C,I2C_EVENT_MASTER_BYTE_TRANSMITTED)));
        if(tmr==0) PCF8591_I2C_Err++;

        I2C_GenerateSTART(PCF8591_I2C, ENABLE);
        tmr = PCF8591_ulTimeOut_Time;
        while((--tmr)&&(!I2C_CheckEvent(PCF8591_I2C, I2C_EVENT_MASTER_MODE_SELECT)));
        if(tmr==0) PCF8591_I2C_Err++;

        I2C_Send7bitAddress(PCF8591_I2C, PCF8591_READ_ADDRESS, I2C_Direction_Receiver);
        tmr = PCF8591_ulTimeOut_Time;
        while((--tmr)&&(!I2C_CheckEvent(PCF8591_I2C, I2C_EVENT_MASTER_RECEIVER_MODE_SELECTED)));
        if(tmr==0) PCF8591_I2C_Err++;

        I2C_AcknowledgeConfig(PCF8591_I2C, DISABLE);
        I2C_GenerateSTOP(PCF8591_I2C, ENABLE);
        tmr = PCF8591_ulTimeOut_Time;
        while((--tmr)&&(!(I2C_CheckEvent(PCF8591_I2C, I2C_EVENT_MASTER_BYTE_RECEIVED))));  /* EV7 */
        if(tmr==0) PCF8591_I2C_Err++;

        readout = I2C_ReceiveData(PCF8591_I2C);

        I2C_AcknowledgeConfig(PCF8591_I2C, ENABLE);
    }
        
    return readout;
}

void PCF8591_WriteDAC(unsigned char DAValue)
{
//  IIC_start();               //启动总线
//   	IIC_write(AddWr);             //发送器件地址
//	if(ack==0)
//		return(0);
//   	IIC_write(0x40);            //发送器件子地址
//    if(ack==0)
//		return(0);
//   	IIC_write(dat);             //发送数据
//    if(ack==0)
//		return(0);
//   	IIC_stop();  
    
//    I2C_WriteOneByte(PCF8591_I2C,PCF8591_WRITE_ADDRESS,0x40,DAValue);

    uint32_t tmr = 0;
    PCF8591_I2C_Err = 0;
    
    tmr = PCF8591_ulTimeOut_Time;
    while(I2C_GetFlagStatus(PCF8591_I2C, I2C_FLAG_BUSY));
    while((--tmr)&&I2C_GetFlagStatus(PCF8591_I2C, I2C_FLAG_BUSY));
    if(tmr==0) PCF8591_I2C_Err ++;

    I2C_GenerateSTART(PCF8591_I2C, ENABLE);
    tmr = PCF8591_ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(PCF8591_I2C, I2C_EVENT_MASTER_MODE_SELECT))); 
    if(tmr==0) PCF8591_I2C_Err ++;

    I2C_Send7bitAddress(PCF8591_I2C, PCF8591_WRITE_ADDRESS, I2C_Direction_Transmitter);
    tmr = PCF8591_ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(PCF8591_I2C, I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED)));
    if(tmr==0) PCF8591_I2C_Err ++;

    I2C_SendData(PCF8591_I2C, 0x40);
    tmr = PCF8591_ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(PCF8591_I2C, I2C_EVENT_MASTER_BYTE_TRANSMITTED)));
    if(tmr==0) PCF8591_I2C_Err ++;

    I2C_SendData(PCF8591_I2C, DAValue);
    tmr = PCF8591_ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(PCF8591_I2C, I2C_EVENT_MASTER_BYTE_TRANSMITTED)));
    if(tmr==0) PCF8591_I2C_Err ++;

    I2C_GenerateSTOP(PCF8591_I2C, ENABLE);
    //I2C_AcknowledgeConfig(I2Cx, DISABLE);
}
