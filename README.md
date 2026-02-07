THARAKA JAYASINGHE
tharaka.info7@gmail.com
+94 766 478 410

# Weather Comfort Dashboard

A full-stack weather dashboard that ranks cities based on a Comfort Index, showing temperature, humidity, wind, cloudiness, and providing visual temperature trends. Built with .NET 8, C#, React, Tailwind CSS, Redis caching, and optionally supports Dark Mode.

Demonstration video: https://drive.google.com/file/d/1prP6i_8_XeKOxWrwAAu3xyiQfPZ6YcZ0/view?usp=sharing

# Setup Instructions

1. Clone the repository

git clone https://github.com/tharaka-jayasinghe-7/WeatherComfortSolution.git

git clone https://github.com/tharaka-jayasinghe-7/WeatherFrontend.git


2. Install backend dependencies

cd WeatherComfortSolution
dotnet restore
dotnet build

3. Install frontend dependencies

cd weather-frontend
npm install

4. Start Redis cache using Docker

Make sure Docker is installed and running, then run:
docker run -d -p 6379:6379 --name weather-redis redis

5. Run the backend

dotnet run

6. Run the frontend

npm run dev

# Comfort Index Formula

* Comfort Index = TempScore + HumidityScore + WindScore + CloudScore

Where:

TempScore = 100 - |Temperature - 22| * 3

HumidityScore = 100 - |Humidity - 50| * 1.5

WindScore = 100 - WindSpeed * 5

CloudScore = 100 - Cloudiness * 0.5

Temperature optimal range: 22°C

Humidity optimal range: 50%

* Scores are capped to ensure they stay between 0 and 100.

* The final comfort score is normalized to a 0–100 range

# Reasoning Behind Variable Weights

* Temperature: Multiplied by 3 to emphasize its importance in comfort perception.

* Humidity: Slightly less impactful, multiplied by 1.5.

* Wind: Moderate effect, multiplied by 5 to penalize very windy conditions.

* Cloudiness: Minimal impact, multiplied by 0.5


# Trade-offs Considered

* Accuracy vs simplicity: The formula is simple for clarity but may not capture microclimate effects.

* Real-time API calls vs caching: Calling OpenWeather API for all cities every page load could be slow; caching with Redis solves this.

* Frontend rendering performance: Sorting/filtering on large datasets could be heavy — handled efficiently with React state and controlled re-renders.

# Cache Design Explanation

Redis is used as a caching layer to store processed weather data (comfort scores) for cities.

* Benefits:

Reduces repeated API calls to OpenWeather

Speeds up dashboard loading

Scales with multiple users

* Implementation:

ICacheService interface abstracts caching logic

RedisCacheService implements Redis operations

* How to check caches?

After login to dashboard,

Open Redis CLI Inside Container

Run:

* docker exec -it redis-cache redis-cli

Check Cached Keys

Inside Redis CLI, run:

* keys weather:*


You should see keys like:

1) "weather:colombo"
2) "weather:tokyo"
3) "weather:sydney"


This confirms the API stored weather responses in Redis.



