# GestureExplorer
## Intro
GestureExplorer is an Immersive Analtycis tool that supports the interactive exploration, classification and sensemaking with large sets of 3D temporal gesture data. GestureExplorer features 3D skeletal and trajectory visualisations of gestures combined with abstract visualisations of clustered sets of gestures. By leveraging the large immersive space afforded by a Virtual Reality interface our tool allows free navigation and control of viewing perspective for users to gain a better understanding of gestures.
Initially conducted as an Honours year project, this project has resulted in 3 publications, inlucding a poster (https://ieeexplore.ieee.org/abstract/document/9757455) and a demo (https://ieeexplore.ieee.org/abstract/document/9757612/) at IEEE VR 2022, and a full paper at ACM CHI 2023 (https://dl.acm.org/doi/abs/10.1145/3544548.3580678).
## How to run GestureExplorer on your local machine
Simply fetch a copy of the project and load it in Unity. Then all you need to do is press the play button. We have tested settings under Unity 2020.3.22f1 with various VR headsets, including Oculus Rift S, Quest, Quest 2, HP Reverb, Samsung Odyssey.

## Example dataset
We used the children whole-body gestures dataset collected by RADU-DANIEL VATAVU. The dataset is avaiable at: http://www.eed.usv.ro/~vatavu/projects/DissimilarityConsensus/
Gesture data in this dataset were recorded in XML format and contained 21 body joints. If you wish to use GestureExplore with data of different format or number of body joints, you may need to configure the scripts as well as related gesture visualisations in Prefabs to adopt the format and number of joints accordingly. An simple usage on Hand gestures could be found in the branch "HandGestureTest" 

