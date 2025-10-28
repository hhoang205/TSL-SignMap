// import { Image } from 'expo-image';
// import { Platform, StyleSheet } from 'react-native';

// import { HelloWave } from '@/components/hello-wave';
// import ParallaxScrollView from '@/components/parallax-scroll-view';
// import { ThemedText } from '@/components/themed-text';
// import { ThemedView } from '@/components/themed-view';
// import { Link } from 'expo-router';

// export default function HomeScreen() {
//   return (
    
//     <ParallaxScrollView
//       headerBackgroundColor={{ light: '#A1CEDC', dark: '#1D3D47' }}
//       headerImage={
//         <Image
//           source={require('@/assets/images/partial-react-logo.png')}
//           style={styles.reactLogo}
//         />
//       }>
//       <ThemedView style={styles.titleContainer}>
//         <ThemedText type="title">Welcome!</ThemedText>
//         <HelloWave />
//       </ThemedView>
//       <ThemedView style={styles.stepContainer}>
//         <ThemedText type="subtitle">Step 1: Try it</ThemedText>
//         <ThemedText>
//           Edit <ThemedText type="defaultSemiBold">app/(tabs)/index.tsx</ThemedText> to see changes.
//           Press{' '}
//           <ThemedText type="defaultSemiBold">
//             {Platform.select({
//               ios: 'cmd + d',
//               android: 'cmd + m',
//               web: 'F12',
//             })}
//           </ThemedText>{' '}
//           to open developer tools.
//         </ThemedText>
//       </ThemedView>
//       <ThemedView style={styles.stepContainer}>
//         <Link href="/modal">
//           <Link.Trigger>
//             <ThemedText type="subtitle">Step 2: Explore</ThemedText>
//           </Link.Trigger>
//           <Link.Preview />
//           <Link.Menu>
//             <Link.MenuAction title="Action" icon="cube" onPress={() => alert('Action pressed')} />
//             <Link.MenuAction
//               title="Share"
//               icon="square.and.arrow.up"
//               onPress={() => alert('Share pressed')}
//             />
//             <Link.Menu title="More" icon="ellipsis">
//               <Link.MenuAction
//                 title="Delete"
//                 icon="trash"
//                 destructive
//                 onPress={() => alert('Delete pressed')}
//               />
//             </Link.Menu>
//           </Link.Menu>
//         </Link>

//         <ThemedText>
//           {`Tap the Explore tab to learn more about what's included in this starter app.`}
//         </ThemedText>
//       </ThemedView>
//       <ThemedView style={styles.stepContainer}>
//         <ThemedText type="subtitle">Step 3: Get a fresh start</ThemedText>
//         <ThemedText>
//           {`When you're ready, run `}
//           <ThemedText type="defaultSemiBold">npm run reset-project</ThemedText> to get a fresh{' '}
//           <ThemedText type="defaultSemiBold">app</ThemedText> directory. This will move the current{' '}
//           <ThemedText type="defaultSemiBold">app</ThemedText> to{' '}
//           <ThemedText type="defaultSemiBold">app-example</ThemedText>.
//         </ThemedText>
//       </ThemedView>
//     </ParallaxScrollView>
//   );
// }

// const styles = StyleSheet.create({
//   titleContainer: {
//     flexDirection: 'row',
//     alignItems: 'center',
//     gap: 8,
//   },
//   stepContainer: {
//     gap: 8,
//     marginBottom: 8,
//   },
//   reactLogo: {
//     height: 178,
//     width: 290,
//     bottom: 0,
//     left: 0,
//     position: 'absolute',
//   },
// });
// import React from 'react';
// import { View, Text, StyleSheet } from 'react-native';

// export default function HomeScreen() {
//   return (
//     <View style={styles.container}>
//       <Text style={styles.text}>Welcome to Home Screen 👋</Text>
//     </View>
//   );
// }

// const styles = StyleSheet.create({
//   container: {
//     flex: 1,
//     alignItems: 'center',
//     justifyContent: 'center',
//     backgroundColor: '#fff',
//   },
//   text: {
//     fontSize: 18,
//     fontWeight: '600',
//   },
// });

// import React from 'react';
// import { NavigationContainer } from '@react-navigation/native';
// import { createStackNavigator, StackNavigationOptions } from '@react-navigation/stack';
// import { StatusBar } from 'expo-status-bar';
// import { AuthProvider, useAuth } from '../../src/contexts/AuthContext';

// // Import screens
// import SplashScreen from '../../src/screens/SplashScreen';
// import LoginScreen from '../../src/screens/auth/LoginScreen';
// import RegisterScreen from '../../src/screens/auth/RegisterScreen';


// // Define navigation parameter types (optional but recommended)
// export type AuthStackParamList = {
//   Login: undefined;
//   Register: undefined;
// };

// export type MainStackParamList = {
//   Home: undefined;
//   Map: undefined;
//   Profile: undefined;
// };

// // Create stacks
// const AuthStack = createStackNavigator<AuthStackParamList>();
// const MainStack = createStackNavigator<MainStackParamList>();

// function AuthStackNavigator() {
//   const screenOptions: StackNavigationOptions = {
//     headerShown: false,
//     cardStyle: { backgroundColor: '#fff' },
//   };

//   return (
//     <AuthStack.Navigator screenOptions={screenOptions}>
//       <AuthStack.Screen name="Login" component={LoginScreen} />
//       <AuthStack.Screen name="Register" component={RegisterScreen} />
//     </AuthStack.Navigator>
//   );
// }

import React, { useState, useEffect } from 'react';
import { StatusBar } from 'expo-status-bar';

// Import screens
import SplashScreen from '../../src/screens/SplashScreen';
import LoginScreen from '../../src/screens/auth/LoginScreen';
import RegisterScreen from '../../src/screens/auth/RegisterScreen';
import HomeScreen from '../../src/screens/main/HomeScreen';
import MapScreen from '../../src/screens/main/MapScreen';
import ProfileScreen from '../../src/screens/main/ProfileScreen';

export default function Index() {
  const [isLoading, setIsLoading] = useState(true);
  const [currentScreen, setCurrentScreen] = useState('Login');

  useEffect(() => {
    // Simulate loading time (3 seconds)
    const timer = setTimeout(() => {
      setIsLoading(false);
    }, 3000);

    return () => clearTimeout(timer);
  }, []);

  const navigation = {
    navigate: (screen) => {
      setCurrentScreen(screen);
    },
    goBack: () => {
      // Simple back navigation logic
      if (currentScreen === 'Register') setCurrentScreen('Login');
      else if (currentScreen === 'Home') setCurrentScreen('Login');
      else if (currentScreen === 'Map') setCurrentScreen('Home');
      else if (currentScreen === 'Profile') setCurrentScreen('Home');
      else setCurrentScreen('Login');
    }
  };

  if (isLoading) {
    return <SplashScreen />;
  }

  // Render current screen based on state
  switch (currentScreen) {
    case 'Login':
      return (
        <>
          <LoginScreen navigation={navigation} />
          <StatusBar style="auto" />
        </>
      );
    case 'Register':
      return (
        <>
          <RegisterScreen navigation={navigation} />
          <StatusBar style="auto" />
        </>
      );
    case 'Home':
      return (
        <>
          <HomeScreen navigation={navigation} />
          <StatusBar style="auto" />
        </>
      );
    case 'Map':
      return (
        <>
          <MapScreen navigation={navigation} />
          <StatusBar style="auto" />
        </>
      );
    case 'Profile':
      return (
        <>
          <ProfileScreen navigation={navigation} />
          <StatusBar style="auto" />
        </>
      );
    default:
      return (
        <>
          <LoginScreen navigation={navigation} />
          <StatusBar style="auto" />
        </>
      );
  }
}

