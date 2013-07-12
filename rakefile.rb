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