@echo off
echo Running Git commits...

git add .
git commit -m "Initial commit: Project structure and basic files"

git add ChatbotEngine.cs
git commit -m "feat: Add ChatbotEngine with keyword recognition"

git add MainWindow.xaml MainWindow.xaml.cs
git commit -m "feat: Implement WPF GUI interface with modern design"

git add VoiceGreeting.cs greeting.wav
git commit -m "feat: Add voice greeting functionality with WAV file"

git add ChatbotEngine.cs
git commit -m "feat: Implement memory system and sentiment detection"

git add ChatbotEngine.cs
git commit -m "feat: Add error handling and random response system"

git add README.md
git commit -m "docs: Add comprehensive README with setup instructions"

git push

echo Done!
pause
