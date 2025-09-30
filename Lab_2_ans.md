# 2 Getting Started
### a) What is the action space?
There is a z and x action. Both continuos and controlls the rotation
Each action is calmped between -1 and 1 and then mulitplied by 2.
### b) What is the state/observation space?
Z rotation
X rotation
ball position relative to gameObject
balls linear velocity
### c) What is the reward function?
While ball on top of the object then the reward is .1
If the ball is not on top of the object the reward is -1 (and the episode ends
### d) What is the model architecture?
hidden_units: 128
num_layers:	2


### e) What are PPO’s hyperparameters (batch size, learning rate, time horizon, max number of steps)?

batch size: 64
learning rate: 0.0003
time horizion: 1000
max number of steps: 500000

# 3 Hyperparameter tuning
### a) Do you see any difference between PPO and SAC (speed, time to converge to a good solution, ...)?
SAC
192000 steps in total
189.366 s after 192000 steps
189.366 s in total

PPO:
492000 steps in total

159.497 s after 192000 steps
344.342 s in total 

PPO is faster per step but needed more steps and time find a good solution

SAC got a mean reward of a 100 after 72000 steps and kept getting a mean reward of a 100 after that.
PPO got a mean reward of a 100 after 132000 steps but did not consistently get a mean rweard of a 100 until after step 432000.

### b) Describe what changed.
With one layer and one neuron it was a bit faster, but it did not learn to solve the task. At the end the mean reward was 1.396, which was similar to the reward in the begining.
So, the network was to small to learn any thing.

### c) Find the smallest neural network (in terms of number of neurons) that can solve this task (get a reward of 100). Write down its architecture (number of layers, number of neurons).
number of layers:1
number of neurons:10

With:
number of layers:1
number of neurons:5

It was able to get a mean reward of a 100 but not consistently and not by the end.

### d) Describe what changed.
With a learning rate of 0.00001:

It gets a better mean reward slower than the original learning rate of 0.0003.

With the lower learning rate there is also a higher chance that it will get stuck at local minimum

It did mange to get a mean reward of a 100 at the very end, but it took more steps.

### e) Describe what changed.
With a learning rate of 0.1:

It gets a worse mean reward. And it never improves.

The learning is too high, which means that it can not converge on the optimal solution, most likely because it keeps "jumping" past it. 

The mean reward stayed around 1
### f) How would you find the best learning rate?
Hyper paramerter tuning. You would have to try out different learning rates, to figure out what works best. There is no one good learning rate. 
# 4 Reward Shaping
### a) Write down the reward function you used (copy-paste the C# code).

1) Close as possible to the center
```cs
if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
{
    SetReward(-1f);
    EndEpisode();
}
else
{
    float distToCenter = Vector2.Distance(ballPos, agentPos);

    if (distToCenter < 0.5f)
    {
        SetReward(1.0f);
    }
    else
    {
        SetReward(0.1f);
    }
}
```
2) Close as possible to one of the corners’ edges
```cs
if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
    Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
    Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
{
    SetReward(-1f);
    EndEpisode();
}
else
{
    float distToCorner = Vector2.Distance(ballPos, agentPos + new Vector2(1.5f, 1.5f));

    if (distToCorner < 0.5f)
    {
        SetReward(1.0f);
    }
    else
    {
        SetReward(0.1f);
    }
}
```
3) Alternates between different corners every 200 steps
```cs
if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
    Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
    Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
{
    SetReward(-1f);
    EndEpisode();
}
else
{
    var cornerIndex = StepCount / 200 % 4;

    var corner = GetCorner((int)cornerIndex);

    float distToAlternateCorner = Vector2.Distance(ballPos, agentPos + corner);

    SetReward(4.5f - distToAlternateCorner);
}
```
I modified the observations to:
```csharp
public override void CollectObservations(VectorSensor sensor)
{
    if (useVecObs)
    {
        var corner = GetCorner(StepCount / 200 % 4);
        sensor.AddObservation(gameObject.transform.rotation.z);
        sensor.AddObservation(gameObject.transform.rotation.x);
        sensor.AddObservation(ball.transform.position - gameObject.transform.position);
        sensor.AddObservation(m_BallRb.linearVelocity);
        sensor.AddObservation(corner);
        sensor.AddObservation(StepCount / 200);
    }
}
```


### b) A 10-second video of your screen showing the learned behaviour (during testing, not learning!). 

