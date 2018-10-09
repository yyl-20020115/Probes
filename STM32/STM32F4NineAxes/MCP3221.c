#include "stm32f4_discovery.h"
#include "MCP3221.h"
#include "SoftI2C.h"


SOFTI2C MCP3221 = {0};


void MCP3221_Init(void)
{    
    /* GPIO Peripheral clock enable */
    RCC_AHB1PeriphClockCmd(MCP3221_GPIO_RCC, ENABLE);
  

    MCP3221.ID = 0;
    MCP3221.SCL_Port= MCP3221_I2C_PORT;
    MCP3221.SDA_Port= MCP3221_I2C_PORT;
    
    MCP3221.SCL_Pin = MCP3221_I2C_SCL_PIN;
    MCP3221.SDA_Pin = MCP3221_I2C_SDA_PIN;
    MCP3221.DelayValue = 1;
    
    SOFTI2C_Init(&MCP3221);
    
}
unsigned char MCP3221_GetState(void)
{
    return MCP3221.State;
}

unsigned short MCP3221_ReadADC(unsigned char Channel)
{
    unsigned short highbyte= 0;

    unsigned short lowbyte = 0;


    if(SOFTI2C_START(&MCP3221) == SOFTI2C_READY)
    {        
        SOFTI2C_SendByte(&MCP3221, MCP3221_READ_ADDRESS);
        
        //SOFTI2C_SendACK(&MCP3221);
        
        highbyte = SOFTI2C_ReceiveByte(&MCP3221);
        
        SOFTI2C_SendACK(&MCP3221);
        
        lowbyte = SOFTI2C_ReceiveByte(&MCP3221);
        
        SOFTI2C_SendNACK(&MCP3221);
        
        
        SOFTI2C_STOP(&MCP3221);
    }
    
    return (((highbyte& 0x0f) <<8) | (lowbyte));
}

