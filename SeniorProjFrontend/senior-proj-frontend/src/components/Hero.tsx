"use client";

import React, {useRef, useState} from 'react'
import { Parallax, ParallaxLayer, IParallax } from '@react-spring/parallax'
import { FaCode } from 'react-icons/fa6'

const url = (name: string, wrap = false) =>
    `${wrap ? 'url(' : ''}https://awv3node-homepage.surge.sh/build/assets/${name}.svg${wrap ? ')' : ''}`

class RandomIcon extends React.Component<{ Icon: any, color?: string }> {
    render() {
        let {Icon, color} = this.props;
        const left = Math.random() * window.innerWidth;

        return (
            <div style={{
                position: 'absolute',
                top: 0,
                left,
                animation: 'fall 5s linear infinite',
                animationDelay: `${Math.random() * 5}s`
            }}>
                <Icon size={30} color={color}/>
            </div>
        );
    }
}


const Hero = () => {
    const parallax = useRef<IParallax>(null!) // parallax element

    const icons = Array.from({ length: 50 }).map((_, i) => <RandomIcon key={i} Icon={FaCode} color="gray" />);

    return (
        <div style={{ width: '100%', height: '100vh', overflow: 'hidden', background: '#253237' }}>
            <Parallax ref={parallax} pages={3}>
                <ParallaxLayer offset={1} speed={1} style={{ backgroundColor: '#805E73' }} />
                <ParallaxLayer offset={2} speed={1} style={{ backgroundColor: '#87BCDE' }} />

                <ParallaxLayer
                    offset={0}
                    speed={0}
                    factor={3}
                    style={{
                        backgroundSize: 'cover',
                    }}
                >
                    {icons}
                </ParallaxLayer>

                <ParallaxLayer offset={1.3} speed={-0.3} style={{ pointerEvents: 'none' }}>
                    <img src={url('satellite4')} style={{ width: '15%', marginLeft: '70%' }}  alt="satellite"/>
                </ParallaxLayer>

                <ParallaxLayer offset={1} speed={0.8} style={{ opacity: 0.1 }}>
                    <img src={url('cloud')} style={{ display: 'block', width: '20%', marginLeft: '55%' }}  alt="cloud"/>
                    <img src={url('cloud')} style={{ display: 'block', width: '10%', marginLeft: '15%' }}  alt="cloud"/>
                </ParallaxLayer>

                <ParallaxLayer offset={1.75} speed={0.5} style={{ opacity: 0.1 }}>
                    <img src={url('cloud')} style={{ display: 'block', width: '20%', marginLeft: '70%' }}  alt="cloud"/>
                    <img src={url('cloud')} style={{ display: 'block', width: '20%', marginLeft: '40%' }}  alt="cloud"/>
                </ParallaxLayer>

                <ParallaxLayer offset={1} speed={0.2} style={{ opacity: 0.2 }}>
                    <img src={url('cloud')} style={{ display: 'block', width: '10%', marginLeft: '10%' }}  alt="cloud"/>
                    <img src={url('cloud')} style={{ display: 'block', width: '20%', marginLeft: '75%' }}  alt="cloud"/>
                </ParallaxLayer>

                <ParallaxLayer offset={1.6} speed={-0.1} style={{ opacity: 0.4 }}>
                    <img src={url('cloud')} style={{ display: 'block', width: '20%', marginLeft: '60%' }}  alt="cloud"/>
                    <img src={url('cloud')} style={{ display: 'block', width: '25%', marginLeft: '30%' }}  alt="cloud"/>
                    <img src={url('cloud')} style={{ display: 'block', width: '10%', marginLeft: '80%' }}  alt="cloud"/>
                </ParallaxLayer>

                <ParallaxLayer offset={2.6} speed={0.4} style={{ opacity: 0.6 }}>
                    <img src={url('cloud')} style={{ display: 'block', width: '20%', marginLeft: '5%' }}  alt="cloud"/>
                    <img src={url('cloud')} style={{ display: 'block', width: '15%', marginLeft: '75%' }}  alt="cloud"/>
                </ParallaxLayer>

                <ParallaxLayer
                    offset={2.5}
                    speed={-0.4}
                    style={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        pointerEvents: 'none',
                    }}>
                    <img src={url('earth')} style={{ width: '60%' }}  alt="earth"/>
                </ParallaxLayer>

                <ParallaxLayer
                    offset={2}
                    speed={-0.3}
                    style={{
                        backgroundSize: '80%',
                        backgroundPosition: 'center',
                        backgroundImage: url('clients', true),
                    }}
                />

                <ParallaxLayer
                    offset={0}
                    speed={0.1}
                    onClick={() => parallax.current.scrollTo(1)}
                    style={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                    }}>
                    <img src={url('server')} style={{ width: '20%' }}  alt="server"/>
                    <div style={{display: 'flex', flexDirection: 'column', alignItems: 'center'}}>
                        <h1 className="text-9xl font-Noto text-blue-200">CodeCoach</h1>
                        <h2 className="text-3xl font-thin text-green-200">Learn to Code with the power of AI</h2>
                    </div>
                </ParallaxLayer>

                <ParallaxLayer
                    offset={1}
                    speed={0.1}
                    onClick={() => parallax.current.scrollTo(2)}
                    style={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                    }}>
                    <img src={url('bash')} style={{ width: '40%' }}  alt="bash"/>
                </ParallaxLayer>

                <ParallaxLayer
                    offset={2}
                    speed={-0}
                    style={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                    }}
                    onClick={() => parallax.current.scrollTo(0)}>
                    <img src={url('clients-main')} style={{ width: '40%' }}  alt="clients-main"/>
                </ParallaxLayer>
            </Parallax>
        </div>
    )
};
export default Hero;
