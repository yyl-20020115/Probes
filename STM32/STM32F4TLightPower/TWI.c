#include "stm32f4xx.h"
#include "stm32f4xx_gpio.h"
#include "TWI.h"


extern void Delay_ms(unsigned int n);


void TWI_SCL_0(void)		{  GPIO_ResetBits(TWI_I2C_PORT,TWI_PIN_SCL);}
void TWI_SCL_1(void) 		{  GPIO_SetBits(TWI_I2C_PORT,TWI_PIN_SCL);}
void TWI_SDA_0(void) 		{  GPIO_ResetBits(TWI_I2C_PORT,TWI_PIN_SDA);}
void TWI_SDA_1(void) 		{  GPIO_SetBits(TWI_I2C_PORT,TWI_PIN_SDA);}


unsigned char   TWI_SDA_STATE(void)        { return (TWI_I2C_PORT->IDR & TWI_PIN_SDA) != 0; }


/*******************************************************************************
* 函数名称:TWI_Delay                                                                     
* 描    述:延时函数                                                                     
*                                                                               
* 输    入:无                                                                     
* 输    出:无                                                                     
* 返    回:无                                                                     
* 作    者:                                                                     
* 修改日期:2010年6月8日                                                                    
*******************************************************************************/
void TWI_NOP(void)
{
    unsigned int i = 20, j = 0;
    unsigned int sum = 0;

    while(i--)
    {
        for (j = 0; j < 10; j++)
        {
            sum += i; 
        }
    }
    sum = i;
}

/*******************************************************************************
* 函数名称:TWI_Initialize                                                                     
* 描    述:I2C初始化函数                                                                     
*                                                                               
* 输    入:无                                                                     
* 输    出:无                                                                     
* 返    回:无                                                                     
* 作    者:                                                                     
* 修改日期:2010年6月8日                                                                    
*******************************************************************************/
void TWI_Initialize(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;
    GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode=GPIO_Mode_OUT;

    GPIO_InitStructure.GPIO_Pin = TWI_PIN_SDA | TWI_PIN_SCL; 
    GPIO_Init(TWI_I2C_PORT, &GPIO_InitStructure);


    TWI_SDA_1();
    TWI_SCL_0(); 
}

/*******************************************************************************
* 函数名称:TWI_START                                                                     
* 描    述:发送启动                                                                     
*                                                                               
* 输    入:无                                                                     
* 输    出:无                                                                     
* 返    回:无                                                                     
* 作    者:                                                                     
* 修改日期:2010年6月8日                                                                    
*******************************************************************************/
unsigned char TWI_START(void)
{ 
    TWI_SDA_1(); 
    TWI_NOP();

    TWI_SCL_1(); 
    TWI_NOP();    

    if(!TWI_SDA_STATE())
    {
        return TWI_BUS_BUSY;
    }
    
    TWI_SDA_0();
    TWI_NOP();

    TWI_SCL_0();  
    TWI_NOP(); 

    if(TWI_SDA_STATE())
    {
        return TWI_BUS_ERROR;
    } 

    return TWI_READY;
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_START_SHT 
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char TWI_START_SHT(void)
{
    TWI_SDA_1();
    TWI_SCL_0();
    TWI_NOP();

    TWI_SDA_1(); 
    TWI_SCL_1(); 
    TWI_NOP();

    if(!TWI_SDA_STATE())
    {
        return TWI_BUS_BUSY;
    }
    
    TWI_SDA_0();
    TWI_NOP();

    TWI_SCL_0();  
    TWI_NOP(); 

    TWI_SCL_1();
    TWI_NOP();

    TWI_SDA_1();
    TWI_NOP();

    TWI_SCL_0();
    TWI_NOP();

    return TWI_READY;
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_STOP 
*/
/* --------------------------------------------------------------------------*/
void TWI_STOP(void)
{
    TWI_SDA_0(); 
    TWI_NOP();

    TWI_SCL_1(); 
    TWI_NOP();    

    TWI_SDA_1();
    TWI_NOP();
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_SendACK 
*/
/* --------------------------------------------------------------------------*/
void TWI_SendACK(void)
{
    TWI_SDA_0();
    TWI_NOP();
    TWI_SCL_1();
    TWI_NOP();
    TWI_SCL_0(); 
    TWI_NOP(); 
    TWI_SDA_1();
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_SendNACK 
*/
/* --------------------------------------------------------------------------*/
void TWI_SendNACK(void)
{
    TWI_SDA_1();
    TWI_NOP();
    TWI_SCL_1();
    TWI_NOP();
    TWI_SCL_0(); 
    TWI_NOP();
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_SendByte 
* 
* @Param: Data
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char TWI_SendByte(unsigned char Data)
{
    unsigned char i = 0;
    
    TWI_SCL_0();
    for(i=0;i<8;i++)
    {  
        //---------数据建立----------
        if(Data&0x80)
        {
            TWI_SDA_1();
        }
        else
        {
            TWI_SDA_0();
        } 
        Data<<=1;
        TWI_NOP();
        //---数据建立保持一定延时----

        //----产生一个上升沿[正脉冲] 
        TWI_SCL_1();
        TWI_NOP();
        TWI_SCL_0();
        TWI_NOP();//延时,防止SCL还没变成低时改变SDA,从而产生START/STOP信号
        //---------------------------   
    }
    
    //接收从机的应答 
    TWI_SDA_1(); 
    TWI_NOP();
    TWI_SCL_1();
    TWI_NOP();   
    
    if(TWI_SDA_STATE())
    {
        TWI_SCL_0();
        TWI_SDA_1();

        return TWI_NACK;
    }
    else
    {
        TWI_SCL_0();
        TWI_SDA_1();
        
        return TWI_ACK;  
    }    
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  TWI_ReceiveByte 
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char TWI_ReceiveByte(void)
{
    unsigned char i = 0,Dat = 0;
    TWI_SDA_1();
    TWI_SCL_0(); 

    for(i=0;i<8;i++)
    {
        TWI_SCL_1();//产生时钟上升沿[正脉冲],让从机准备好数据 
        TWI_NOP(); 
        Dat<<=1;
        if(TWI_SDA_STATE()) //读引脚状态
        {
            Dat|=0x01; 
        }   
        TWI_SCL_0();//准备好再次接收数据  
        TWI_NOP();//等待数据准备好         
    }
    return Dat;
}
