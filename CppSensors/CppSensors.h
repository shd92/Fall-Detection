#pragma once

using namespace Windows::Devices::Sensors;
using namespace Windows::Foundation;

namespace CppSensors
{
	public delegate void AccelerometerEvent(double x, double y, double z);
	public delegate void GyroscopeEvent(double x, double y, double z);
	public delegate void CompassEvent(double n);

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


	public ref class CppCompass sealed
	{
	private:
		Compass ^compass;
		double n = 0;

	public:
		CppCompass();

		//Event that we trigger when we have a new reading
		event CompassEvent^ onReadingChanged;

		//Internal function that gets called by the compass thread when it has a new reading for us
		void compassChanged(Compass ^sender, CompassReadingChangedEventArgs ^args);

		double getN();

		virtual ~CppCompass();
	};
}