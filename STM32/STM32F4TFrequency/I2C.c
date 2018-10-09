#include "stm32f4_discovery.h"
#include "I2C.h"

unsigned int ulTimeOut_Time = 0;
unsigned char I2C_Err=0;


unsigned char I2C_GetErrorCode(void)
{
    return I2C_Err;
}


void I2C_Config(void)
{
    GPIO_InitTypeDef    GPIO_InitStructure;
    I2C_InitTypeDef     I2C_InitStructure;
    RCC_ClocksTypeDef   rcc_clocks;

    /* GPIO Peripheral clock enable */
    RCC_AHB1PeriphClockCmd(RCC_AHB1Periph_GPIOA|RCC_AHB1Periph_GPIOB|RCC_AHB1Periph_GPIOC, ENABLE);
    RCC_APB1PeriphClockCmd(RCC_APB1Periph_I2C1|RCC_APB1Periph_I2C2|RCC_APB1Periph_I2C3, ENABLE);
      /* Reset I2Cx IP */
    RCC_APB1PeriphResetCmd(RCC_APB1Periph_I2C1|RCC_APB1Periph_I2C2|RCC_APB1Periph_I2C3, ENABLE);
    /* Release reset signal of I2Cx IP */
    RCC_APB1PeriphResetCmd(RCC_APB1Periph_I2C1|RCC_APB1Periph_I2C2|RCC_APB1Periph_I2C3, DISABLE);

    /*I2C1 configuration*/
    GPIO_PinAFConfig(GPIOB, GPIO_PinSource6, GPIO_AF_I2C1); //注意，此处不能合并写成GPIO_PinSource6|GPIO_PinSource7
    GPIO_PinAFConfig(GPIOB, GPIO_PinSource7, GPIO_AF_I2C1);

    //PB6: I2C1_SCL  PB7: I2C1_SDA
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_6|GPIO_Pin_7;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_OD;
    GPIO_InitStructure.GPIO_PuPd  = GPIO_PuPd_NOPULL;
    GPIO_Init(GPIOB, &GPIO_InitStructure);

    /* I2C Struct Initialize */
    I2C_DeInit(I2C1);
    I2C_InitStructure.I2C_Mode = I2C_Mode_I2C;
    I2C_InitStructure.I2C_DutyCycle = I2C_DutyCycle_2;
    I2C_InitStructure.I2C_OwnAddress1 = 0x00;
    I2C_InitStructure.I2C_Ack = I2C_Ack_Enable;
    I2C_InitStructure.I2C_ClockSpeed = I2C_CLOCK_SPEED;
    I2C_InitStructure.I2C_AcknowledgedAddress = I2C_AcknowledgedAddress_7bit;
    I2C_Init(I2C1, &I2C_InitStructure);

    /* I2C Initialize */
    I2C_Cmd(I2C1, ENABLE);

    /*I2C2 configuration*/
    GPIO_PinAFConfig(GPIOB, GPIO_PinSource10, GPIO_AF_I2C2); //注意，此处不能合并写成GPIO_PinSource6|GPIO_PinSource7
    GPIO_PinAFConfig(GPIOB, GPIO_PinSource11, GPIO_AF_I2C2);

    //PB10: I2C2_SCL  PB11: I2C2_SDA
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10|GPIO_Pin_11;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_OD;
    GPIO_InitStructure.GPIO_PuPd  = GPIO_PuPd_NOPULL;
    GPIO_Init(GPIOB, &GPIO_InitStructure);

    /* I2C Struct Initialize */
    I2C_DeInit(I2C2);
    I2C_InitStructure.I2C_Mode = I2C_Mode_I2C;
    I2C_InitStructure.I2C_DutyCycle = I2C_DutyCycle_2;
    I2C_InitStructure.I2C_OwnAddress1 = 0x00;
    I2C_InitStructure.I2C_Ack = I2C_Ack_Enable;
    I2C_InitStructure.I2C_ClockSpeed = I2C_CLOCK_SPEED;
    I2C_InitStructure.I2C_AcknowledgedAddress = I2C_AcknowledgedAddress_7bit;
    I2C_Init(I2C2, &I2C_InitStructure);

    /* I2C Initialize */
    I2C_Cmd(I2C2, ENABLE);

    /*I2C3 configuration*/
    GPIO_PinAFConfig(GPIOA, GPIO_PinSource8, GPIO_AF_I2C3);
    GPIO_PinAFConfig(GPIOC, GPIO_PinSource9, GPIO_AF_I2C3);

    //PA8: I2C3_SCL
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_8;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_OD;
    GPIO_InitStructure.GPIO_PuPd  = GPIO_PuPd_NOPULL;
    GPIO_Init(GPIOA, &GPIO_InitStructure);

    //PC9: I2C3_SDA
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9;
    GPIO_Init(GPIOC, &GPIO_InitStructure);

    /* I2C Struct Initialize */
    I2C_DeInit(I2C3);
    I2C_InitStructure.I2C_Mode = I2C_Mode_I2C;
    I2C_InitStructure.I2C_DutyCycle = I2C_DutyCycle_2;
    I2C_InitStructure.I2C_OwnAddress1 = 0x00;
    I2C_InitStructure.I2C_Ack = I2C_Ack_Enable;
    I2C_InitStructure.I2C_ClockSpeed = I2C_CLOCK_SPEED;
    I2C_InitStructure.I2C_AcknowledgedAddress = I2C_AcknowledgedAddress_7bit;
    I2C_Init(I2C3, &I2C_InitStructure);

    /* I2C Initialize */
    I2C_Cmd(I2C3, ENABLE);


    /*超时设置*/
    RCC_GetClocksFreq(&rcc_clocks);
    ulTimeOut_Time = (rcc_clocks.SYSCLK_Frequency /10000); 
}


unsigned char I2C_ReadOneByte(I2C_TypeDef *I2Cx,unsigned char I2C_Addr,unsigned char Reg_addr)
{  
    uint8_t readout = 0;
    uint32_t tmr = 0;
    I2C_Err = 0;

    tmr = ulTimeOut_Time;
    while((--tmr)&&I2C_GetFlagStatus(I2Cx, I2C_FLAG_BUSY));
    if(tmr==0) I2C_Err++;

    I2C_GenerateSTART(I2Cx, ENABLE);
    //发送I2C的START信号，接口自动从从设备编程主设备
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(I2Cx, I2C_EVENT_MASTER_MODE_SELECT)));
    if(tmr==0) I2C_Err++;

    I2C_Send7bitAddress(I2Cx,I2C_Addr,I2C_Direction_Transmitter);
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(I2Cx,I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED)));
    if(tmr==0) I2C_Err++;

    I2C_SendData(I2Cx, Reg_addr);
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(I2Cx,I2C_EVENT_MASTER_BYTE_TRANSMITTED)));
    if(tmr==0) I2C_Err++;

    I2C_GenerateSTART(I2Cx, ENABLE);
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(I2Cx, I2C_EVENT_MASTER_MODE_SELECT)));
    if(tmr==0) I2C_Err++;

    I2C_Send7bitAddress(I2Cx, I2C_Addr, I2C_Direction_Receiver);
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(I2Cx, I2C_EVENT_MASTER_RECEIVER_MODE_SELECTED)));
    if(tmr==0) I2C_Err++;

    I2C_AcknowledgeConfig(I2Cx, DISABLE);
    I2C_GenerateSTOP(I2Cx, ENABLE);
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!(I2C_CheckEvent(I2Cx, I2C_EVENT_MASTER_BYTE_RECEIVED))));  /* EV7 */
    if(tmr==0) I2C_Err++;

    readout = I2C_ReceiveData(I2Cx);

    I2C_AcknowledgeConfig(I2Cx, ENABLE);

    return readout;
}

void I2C_WriteOneByte(I2C_TypeDef *I2Cx,uint8_t I2C_Addr,uint8_t Reg_addr,uint8_t value)
{
    uint32_t tmr = 0;
    I2C_Err = 0;

    tmr = ulTimeOut_Time;
    while(I2C_GetFlagStatus(I2Cx, I2C_FLAG_BUSY));
    while((--tmr)&&I2C_GetFlagStatus(I2Cx, I2C_FLAG_BUSY));
    if(tmr==0) I2C_Err++;

    I2C_GenerateSTART(I2Cx, ENABLE);
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(I2Cx, I2C_EVENT_MASTER_MODE_SELECT))); 
    if(tmr==0) I2C_Err++;

    I2C_Send7bitAddress(I2Cx, I2C_Addr, I2C_Direction_Transmitter);
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(I2Cx, I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED)));
    if(tmr==0) I2C_Err++;

    I2C_SendData(I2Cx, Reg_addr);
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(I2Cx, I2C_EVENT_MASTER_BYTE_TRANSMITTED)));
    if(tmr==0) I2C_Err++;

    I2C_SendData(I2Cx, value);
    tmr = ulTimeOut_Time;
    while((--tmr)&&(!I2C_CheckEvent(I2Cx, I2C_EVENT_MASTER_BYTE_TRANSMITTED)));
    if(tmr==0) I2C_Err++;

    I2C_GenerateSTOP(I2Cx, ENABLE);
    //I2C_AcknowledgeConfig(I2Cx, DISABLE);
}

