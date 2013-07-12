begin
  require 'bundler/setup'
  require 'fuburake'
  require "rubygems/package"
  require 'rake/packagetask'
  require "rexml/document"
rescue LoadError
  puts 'Bundler and all the gems need to be installed prior to running this rake script. Installing...'
  system("gem install bundler --source http://rubygems.org")
  sh 'bundle install'
  system("bundle exec rake", *ARGV)
  exit 0
end

solution = FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/TikaOnDotnet.sln'
	}

=begin
	sln.assembly_info = {
		:product_name => "TikaOnDotnet",
		:copyright => 'Copyright Kevin Miller 2013',
		:output_file => 'src/CommonAssemblyInfo.cs'
	}
=end

	sln.ripple_enabled = true
end


=begin
require 'rubygems'
require 'albacore'

# Common Settings
CLR_VERSION = "v4.0.30319"
COMPILE_TARGET = "Debug"
ROOT_NAMESPACE = "TikaOnDotnet"

desc "Compiles and runs unit tests"
task :all => [:default]

desc "**Default**, compiles and runs tests"
task :default => [:compile, :test]

task :compile do
	compile_solution("#{ROOT_NAMESPACE}.sln", "debug")
end

def compile_solution(solution, configuration)

	puts("building #{solution} with configuration #{configuration}")

	msb = MSBuild.new
	msb.properties :configuration => configuration
	msb.solution = solution
	msb.targets [:Clean, :Build]	
	msb.execute 
end

desc "Runs unit tests"
task :test => [:nunit]

nunit do |nunit|
	nunit.command = "tools/NUnit/nunit-console.exe"
	nunit.assemblies "TikaOnDotnet/bin/Debug/TikaOnDotnet.dll"
end

=end
