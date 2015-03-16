#pragma once

using namespace Windows::Devices::Sensors;
using namespace Windows::Foundation;

namespace CppSensors
{
	public delegate void AccelerometerEvent(double x, double y, double z);
	public delegate void GyroscopeEvent(double x, double y, double z);
	public delegate void GyroAccEvent(double accX, double accY, double accZ, double gyrX, double gyrY, double gyrZ);

	//Accelerometer and Gyroscope Sensor tool
	public ref class CppAccGyro sealed
	{
	private:
		Accelerometer ^acc;
		Gyrometer ^gyro;
		double AccX, AccY, AccZ;
		double GyrX, GyrY, GyrZ;

	public:
		CppAccGyro();
		event GyroAccEvent^ onReadingChanged;
		void accChanged(Accelerometer ^sender, AccelerometerReadingChangedEventArgs ^args);
		void gyroChanged(Gyrometer ^sender, GyrometerReadingChangedEventArgs ^args);
		virtual ~CppAccGyro();
	};

	//Acceleromter Sensor Tool
	public ref class CppAcc sealed
	{
	private:
		Accelerometer ^acc;

	public:
		CppAcc();
		event AccelerometerEvent^ onReadingChanged;
		void accChanged(Accelerometer ^sender, AccelerometerReadingChangedEventArgs ^args);
		virtual ~CppAcc();

	};

	//Gyrometer Sensor Tool
	public ref class CppGyro sealed
	{
	private:
		Gyrometer ^gyro;

	public:
		CppGyro();
		event GyroscopeEvent^ onReadingChanged;
		void gyroChanged(Gyrometer ^sender, GyrometerReadingChangedEventArgs ^args);
		virtual ~CppGyro();
	};

}