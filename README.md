# Procedural Environment Generation

##Approach to procedural environment generation and realistic agents behaviour in Unity 4

Mathieu Boucher

Yanis Hattab

Implemented for the Video Games course (COMP 521) at the McGill University School
of Computer Science. 

[View Report](ProceduralEnvironmentGeneration/ProjectReport.pdf)

In recent years, modern video games have improved exponentially in terms of the size of
the world they offer and its fidelity to reality. Games like Assassin’s Creed Black Flag or Just
Cause 2 offer thousands of kilometers of environment to explore and its realism is sometimes
striking. Such virtual worlds are expensive to make and are content heavy, we aim to ease such
projects by building a framework that automatically generates realistic environments in a
procedural way while maintaining high fidelity and flexibility for the game designers. We want
to be able to generate a 3D terrain randomly and populate it with realistically behaving agents
(animals for now) that would use steering behaviours to display improvisational and natural
movements.

##Terrain Generation
Algorithms to produce realistic terrain in a procedural context are useful for many
applications beside video games and many of theses algorithms generate terrain without any
userinput
or interaction to customize it, while the terrain produced by the latter mean may be
realistic, the designer is not able to specify certain key features in the generated terrain. In our
project, we explored a way for the user to be able to seed a height map to set key points in the
terrain instead of relying on a semirandom
seeded height map (though we also cover this). This
allows the user to create terrain with specific properties. The terrain generation implementation
and the report sections on it are mainly done by Mathieu Boucher although it is happening in a
spirit of collaboration.
##Indigenous agentsè AI
Steering behaviours have been around in the video game industry for a while and
represent a simple and efficient way to produce emergent behaviour and have nondeterministic
AI. In our case we want to aggregate the different steering behaviours in one single base class
that will be extended by agents so as to have the game designer only think of higher order goals
for his agents and leave the locomotion primitives to the base class. The steering behaviours’
implementation and report sections are done by Yanis Hattab with their inclusion in the terrain
done and tested cooperatively.