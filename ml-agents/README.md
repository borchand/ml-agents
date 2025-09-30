# My additions 
### Training with ML-Agents
```
// ppo
mlagents-learn config/ppo/3DBall.yaml --run-id=<run_id> 
```
### Inference with ML-Agents (run trained model in Unity)
```
mlagents-learn config/ppo/3DBall.yaml --resume --run-id=<run_id> --inference
```

After running the above command, press the play button in the Unity Editor to see the trained model in action.


### Trained model ids
| Description                                | Run ID                  |
|--------------------------------------------|-------------------------|
| 3DBall PPO - Keep ball in center           | 3DBallRunCenter         |
| 3DBall PPO - Keep ball in top right corner | 3DBallRunTopRightCorner |
| 3DBall PPO - Alternate between corners  | 3DBallRunCorners        |




# Unity ML-Agents Trainers

The `mlagents` Python package is part of the
[ML-Agents Toolkit](https://github.com/Unity-Technologies/ml-agents). `mlagents`
provides a set of reinforcement and imitation learning algorithms designed to be
used with Unity environments. The algorithms interface with the Python API
provided by the `mlagents_envs` package. See [here](../com.unity.ml-agents/Documentation~/Python-LLAPI.md) for
more information on `mlagents_envs`.

The algorithms can be accessed using the: `mlagents-learn` access point. See
[here](../com.unity.ml-agents/Documentation~/Training-ML-Agents.md) for more information on using this
package.

## Installation

Install the `mlagents` package with:

```sh
python -m pip install mlagents==1.1.0
```

## Usage & More Information

For more information on the ML-Agents Toolkit and how to instrument a Unity
scene with the ML-Agents SDK, check out the main
[ML-Agents Toolkit documentation](https://docs.unity3d.com/Packages/com.unity.ml-agents@latest).

## Limitations

- Resuming self-play from a checkpoint resets the reported ELO to the default
  value.
