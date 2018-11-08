#include "STM32_I2C.h"
#include  "Compass.h"
#include "delay.h"
#include "led.h"



#define AKM_REG_WHOAMI      (0x00)

#define AKM_REG_ST1         (0x02)
#define AKM_REG_HXL         (0x03)
#define AKM_REG_ST2         (0x09)

#define AKM_REG_CNTL        (0x0A)
#define AKM_REG_ASTC        (0x0C)
#define AKM_REG_ASAX        (0x10)
#define AKM_REG_ASAY        (0x11)
#define AKM_REG_ASAZ        (0x12)

#define AKM_DATA_READY      (0x01)
#define AKM_DATA_OVERRUN    (0x02)
#define AKM_OVERFLOW        (0x80)
#define AKM_DATA_ERROR      (0x40)

#define AKM_BIT_SELF_TEST   (0x40)

#define AKM_POWER_DOWN          (0x00 | SUPPORTS_AK89xx_HIGH_SENS)
#define AKM_SINGLE_MEASUREMENT  (0x01 | SUPPORTS_AK89xx_HIGH_SENS)
#define AKM_FUSE_ROM_ACCESS     (0x0F | SUPPORTS_AK89xx_HIGH_SENS)
#define AKM_MODE_SELF_TEST      (0x08 | SUPPORTS_AK89xx_HIGH_SENS)

#define AKM_WHOAMI      (0x48)

#define USET_CTR            (0x6A)
#define INT_PIN_CFG			(0x37)
#define RAW_COMPASS			(0X49)
#define COMPASS_ADDR	    (0X18)
#define BIT_AUX_IF_EN       (0x20)
#define BIT_BYPASS_EN       (0x02)
#define BIT_ACTL            (0x80)
#define INV_XYZ_COMPASS     (0x01)
#define BIT_BYPASS_EN       (0x02)
#define BIT_LATCH_EN        (0x20)
#define BIT_ANY_RD_CLR      (0x10)

#define MPU9150_RA_MAG_XOUT_L		0x03

#define MPU9150_RA_INT_PIN_CFG      0x37

#define MPU6050_RA_USER_CTRL        0x6A

#define MPU6050_INTCFG_I2C_BYPASS_EN_BIT   1


#define MPU6050_USERCTRL_I2C_MST_EN_BIT         5


unsigned char bypass_mode=0XFF;
unsigned int sensors=0XFF; 
float  data[3];
float  mag[3];
float  init_mx ;                                                
float  init_my ;
float  init_mz ;

 /**
 *  @brief      Set device to bypass mode.
 *  @param[in]  bypass_on   1 to enable bypass mode.
 *  @return     0 if successful.
 */

 int mpu_set_bypass(unsigned char bypass_on)
{
    unsigned char enable;
	enable=	bypass_on;
    if(bypass_on)
	{ 
      IIC_write(MPU9150_Addr, MPU9150_RA_INT_PIN_CFG, MPU6050_INTCFG_I2C_BYPASS_EN_BIT, &enable);  
	  LED0=!LED0;
		 }
	else
	{
      	IIC_write(MPU9150_Addr, MPU9150_RA_INT_PIN_CFG, MPU6050_INTCFG_I2C_BYPASS_EN_BIT, 0);  
	    LED2=!LED2;
	} 
   return 0;
} 	
/**
 *  @brief      Read raw compass data.
 *  @param[out] data        Raw data in hardware units.
 *  @param[out] timestamp   Timestamp in milliseconds. Null if not needed.
 *  @return     0 if successful.
 */	
int mpu_get_compass_reg()
{

	unsigned char buffer[6];
   	Single_Write(MPU9150_Addr, MPU9150_RA_INT_PIN_CFG, 0x02); //set i2c bypass enable pin to true to access magnetometer
	delay_ms(10);
	Single_Write(COMPASS_ADDR, 0x0A, 0x01); //enable the magnetometer
	delay_ms(10);
	I2cRead(COMPASS_ADDR, MPU9150_RA_MAG_XOUT_L, 6, buffer);
	mag[0] = (((int16_t)buffer[0]) << 8) | buffer[1];
    mag[1] = (((int16_t)buffer[2]) << 8) | buffer[3];
    mag[2] = (((int16_t)buffer[4]) << 8) | buffer[5];

  //  mag[0] = ((long)data[0] * mag_sens_adj[0]) >> 8;
  //  mag[1] = ((long)data[1] * mag_sens_adj[1]) >> 8;
  //  mag[2] = ((long)data[2] * mag_sens_adj[2]) >> 8;
    //init_mx =(float)1.046632*data[0]-1.569948;                                                
   // init_my =(float)data[1]-8;
    //init_mz =(float)-data[2];
    return 0; 
} 
	
 void mpu_compass_value_get()
 {
     unsigned char tmp=1;
 	 IIC_write(MPU9150_Addr, MPU6050_RA_USER_CTRL, MPU6050_USERCTRL_I2C_MST_EN_BIT, 0);
	 delay_ms(20);
     mpu_set_bypass(1);           //开启bypass    
     mpu_get_compass_reg();
	 IIC_write(MPU9150_Addr, MPU6050_RA_USER_CTRL, MPU6050_USERCTRL_I2C_MST_EN_BIT, &tmp);
	 delay_ms(20);    
     mpu_set_bypass(0); 

 }
/* void ANBT_MPU9150_I2CBYPASS_CFG_FUN(void)   
{
        I2C_Start();
        I2C_SendByte(COMPASS_ADDR);                                        //圆点博士:发送陀螺仪写地址
        I2C_SendByte(MPU9150_RA_INT_PIN_CFG);   //圆点博士:发送陀螺仪PWM地址
        I2C_SendByte(1);         //圆点博士:发送陀螺仪PWM值
        I2C_Stop();
}

 void ANBT_MPU6050_I2CBYPASS_CFG_FUN(void)   
{
        I2C_Start();
        I2C_SendByte(COMPASS_ADDR);                                        //圆点博士:发送陀螺仪写地址
        I2C_SendByte(MPU9150_RA_INT_PIN_CFG);   //圆点博士:发送陀螺仪PWM地址
        I2C_SendByte(0);         //圆点博士:发送陀螺仪PWM值
        I2C_Stop();	 
} */
/* u8 ANBT_AK8975_MAG_WHOAMI_FUN(void)
{
        u8 anbt_ak8975_mag_id;
        //
        I2C_Start();
        I2C_SendByte(COMPASS_ADDR);                        //圆点博士:发送陀螺仪写地址
        I2C_SendByte(AKM_WHOAMI);  //圆点博士:发送陀螺仪ID地址
        I2C_Start();
        I2C_SendByte(COMPASS_ADDR+1);      //圆点博士:发送陀螺仪读地址
        anbt_ak8975_mag_id=I2C_ReceiveByte();                                //圆点博士:读出陀螺仪ID
        I2C_Stop();
        //
        return anbt_ak8975_mag_id;
}*/	
  /*
	#define MPU6050_RA_INT_PIN_CFG      0x37

#define MPU9150_RA_MAG_XOUT_L		0x03

	unsigned char buffer[6];
   	Single_Write(MPU9150_Addr, MPU9150_RA_INT_PIN_CFG, 0x02); //set i2c bypass enable pin to true to access magnetometer
	delay_ms(10);
	Single_Write(COMPASS_ADDR, 0x0A, 0x01); //enable the magnetometer
	delay_ms(10);
	I2cRead(COMPASS_ADDR, MPU9150_RA_MAG_XOUT_L, 6, buffer);
	mag[0] = (((int16_t)buffer[0]) << 8) | buffer[1];
    mag[1] = (((int16_t)buffer[2]) << 8) | buffer[3];
    mag[2] = (((int16_t)buffer[4]) << 8) | buffer[5];	
  */
    /* unsigned char tmp;
    if (bypass_mode == bypass_on)
        return 0;

    if (bypass_on) {
        if(I2cRead(MPU9150_Addr, USET_CTR, 1, &tmp))
		return -1;
        tmp &= ~BIT_AUX_IF_EN;
        if(IIC_write(MPU9150_Addr, USET_CTR, 1, &tmp))
		return -1;
        delay_ms(3);
        tmp = BIT_BYPASS_EN;
        if (active_low_int)
            tmp |= BIT_ACTL;
        if (latched_int)
            tmp |= BIT_LATCH_EN | BIT_ANY_RD_CLR;
        if(IIC_write(MPU9150_Addr, INT_PIN_CFG, 1, &tmp))
		return -1;
		LED0=!LED0;
    } else { 

        if(I2cRead(MPU9150_Addr, USET_CTR, 1, &tmp))
	     	return -1;

        if (sensors & INV_XYZ_COMPASS)
            tmp |= BIT_AUX_IF_EN;
        else
            tmp &= ~BIT_AUX_IF_EN;
        if(IIC_write(MPU9150_Addr, USET_CTR, 1, &tmp))
            return -1;
        delay_ms(3);
        if (active_low_int)
            tmp = BIT_ACTL;
        else
            tmp = 0;
        if (latched_int)
            tmp |= BIT_LATCH_EN | BIT_ANY_RD_CLR;
        if(IIC_write(MPU9150_Addr, INT_PIN_CFG, 1, &tmp))
            return -1;
			LED2=!LED2;
    }
    bypass_mode = bypass_on;
    return 0; 	
	*/