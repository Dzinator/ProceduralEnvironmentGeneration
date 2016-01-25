# Procedural Environment Generation

##Approach to procedural environment generation and realistic agents behaviour in Unity 4

Mathieu Boucher
Yanis Hattab

Implemented for the Video Games course (COMP 521) at the McGill University School
of Computer Science. 

[View Report](ProceduralEnvironmentGeneration/blob/master/ProjectReport.pdf)

In recent years, modern video games have improved exponentially in terms of the size of
the world they offer and its fidelity to reality. Games like Assassin’s Creed Black Flag or Just
Cause 2 offer thousands of kilometers of environment to explore and its realism is sometimes
striking. Such virtual worlds are expensive to make and are content heavy, we aim to ease such
projects by building a framework that automatically generates realistic environments in a
procedural way while maintaining high fidelity and flexibility for the game designers. We want
to be able to generate a 3D terrain randomly and populate it with realistically behaving agents
(animals for now) that would use steering behaviours to display improvisational and natural
movements.