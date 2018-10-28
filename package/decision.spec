Name:           DecisionMapperWebApi
Version:        [version]
Release:        0%{?dist}
Summary:        DecisionWebApi test

License:        GPLv3
URL:            https://github.com/ArtemStepanov/%{name}
Source:         https://github.com/ArtemStepanov/%{name}/archive/v%{version}.zip

BuildArch:      noarch
BuildRoot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)

BuildRequires:  dotnet-sdk-2.1
Requires:       dotnet-sdk-2.1

%description
Test project for decision mapper

%prep
%setup -q

%build
rm -rf %{buildroot}

%install
mkdir -p %{buildroot}/usr/lib/%{name}
mkdir -p %{buildroot}%{_bindir}

dotnet publish -c Release -o %{buildroot}/usr/lib/%{name}

cat > %{buildroot}%{_bindir}/%{name} <<-EOF
#!/bin/bash
%{_bindir}/dotnet /usr/lib/%{name}/DecisionWebApi.dll
EOF

chmod 0755 %{buildroot}%{_bindir}/%{name}

%files
/usr/lib/%{name}/*
%{_bindir}/%{name}

%changelog
* Fri Oct 26 2018 Artem Stepanov <stxima@gmail.com> - 1.0
- Initial build