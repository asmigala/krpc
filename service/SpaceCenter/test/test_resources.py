import unittest
import testingtools
import krpc

class ResourcesTest(object):

    density = {
        'MonoPropellant': 4,
        'LiquidFuel':     5,
        'Oxidizer':       5,
        'SolidFuel':      7.5
    }

class TestResources(testingtools.TestCase, ResourcesTest):

    @classmethod
    def setUpClass(cls):
        testingtools.new_save()
        testingtools.launch_vessel_from_vab('Resources')
        cls.conn = testingtools.connect(name='TestResources')
        cls.vessel = cls.conn.space_center.active_vessel
        cls.num_stages = len(cls.expected.keys())

    @classmethod
    def tearDownClass(cls):
        cls.conn.close()

    expected = {
        0: {
            'ElectricCharge': (150, 150),
            'MonoPropellant': (15, 30),
            'LiquidFuel':     (0, 0),
            'Oxidizer':       (0, 0),
            'SolidFuel':      (0, 0)
        },
        1: {
            'ElectricCharge': (0, 0),
            'MonoPropellant': (0, 0),
            'LiquidFuel':     (0, 0),
            'Oxidizer':       (0, 0),
            'SolidFuel':      (0, 0)
        },
        2: {
            'ElectricCharge': (0, 0),
            'MonoPropellant': (0, 0),
            'LiquidFuel':     (720, 1440),
            'Oxidizer':       (1000, 1760),
            'SolidFuel':      (0, 0)
        },
        3: {
            'ElectricCharge': (0, 0),
            'MonoPropellant': (0, 0),
            'LiquidFuel':     (0, 0),
            'Oxidizer':       (0, 0),
            'SolidFuel':      (0, 0)
        },
        4: {
            'ElectricCharge': (0, 0),
            'MonoPropellant': (0, 0),
            'LiquidFuel':     (720+720+300, 1440+1440+720),
            'Oxidizer':       (1000+1000+400, 1760+1760+880),
            'SolidFuel':      (13, 15)
        },
        5: {
            'ElectricCharge': (0, 0),
            'MonoPropellant': (0, 0),
            'LiquidFuel':     (0, 0),
            'Oxidizer':       (0, 0),
            'SolidFuel':      (300*4 + 3*8, 850*4 + 8*8)
        },
    }

    expected_names = {
        0: set(['ElectricCharge','MonoPropellant']),
        1: set(),
        2: set(['LiquidFuel','Oxidizer','ElectricCharge']),
        3: set(),
        4: set(['LiquidFuel','Oxidizer','ElectricCharge','SolidFuel']),
        5: set(['SolidFuel'])
    }

    expected_names_cumulative = {
        0: set(['ElectricCharge','MonoPropellant']),
        1: set(['ElectricCharge','MonoPropellant']),
        2: set(['ElectricCharge','MonoPropellant','LiquidFuel','Oxidizer']),
        3: set(['ElectricCharge','MonoPropellant','LiquidFuel','Oxidizer']),
        4: set(['ElectricCharge','MonoPropellant','LiquidFuel','Oxidizer','SolidFuel']),
        5: set(['ElectricCharge','MonoPropellant','LiquidFuel','Oxidizer','SolidFuel'])
    }

    def test_equality(self):
        self.assertEqual(self.vessel.resources, self.vessel.resources)

    def test_names(self):
        self.assertEqual(
            set(['ElectricCharge', 'MonoPropellant', 'LiquidFuel', 'Oxidizer', 'SolidFuel']),
            set(self.vessel.resources.names))

    def test_per_stage_amounts(self):
        for stage in range(self.num_stages):
            resources = self.vessel.resources_in_decouple_stage(stage=stage, cumulative=False)
            self.assertEqual(self.expected_names[stage], set(resources.names))
            for name in resources.names:
                name = str(name) #TODO: remove str(.)
                self.assertTrue(resources.has_resource(name))
                self.assertClose(self.expected[stage][name][0], resources.amount(name), error=0.5)
                self.assertClose(self.expected[stage][name][1], resources.max(name), error=0.5)

    def test_per_stage_amounts_cumulative(self):
        for stage in range(self.num_stages):
            resources = self.vessel.resources_in_decouple_stage(stage=stage)
            self.assertEqual(self.expected_names_cumulative[stage], set(resources.names))
            for name in resources.names:
                name = str(name) #TODO: remove str(.)
                expected_amount = sum(self.expected[x][name][0] for x in range(stage+1))
                expected_max = sum(self.expected[x][name][1] for x in range(stage+1))
                self.assertClose(expected_amount, resources.amount(name), error=0.5)
                self.assertClose(expected_max, resources.max(name), error=0.5)

    def test_total_amounts(self):
        resources = self.vessel.resources
        self.assertEqual(
            set(['SolidFuel','ElectricCharge','MonoPropellant','LiquidFuel','Oxidizer']),
            set(resources.names))
        for name in resources.names:
            name = str(name) #TODO: remove str(.)
            expected_amount = sum(self.expected[stage][name][0] for stage in range(self.num_stages))
            expected_max = sum(self.expected[stage][name][1] for stage in range(self.num_stages))
            self.assertClose(expected_amount, resources.amount(name), error=0.5)
            self.assertClose(expected_max, resources.max(name), error=0.5)

    def test_vessel_mass(self):
        mass = self.vessel.dry_mass
        self.assertEquals(28595, mass)
        resources = self.vessel.resources
        self.assertEqual(
            set([u'SolidFuel','ElectricCharge','MonoPropellant','LiquidFuel','Oxidizer']),
            set(resources.names))
        for name in resources.names:
            amount = sum(self.expected[stage][name][0] for stage in range(self.num_stages))
            if name in self.density:
                mass += amount * self.density[name]
        self.assertEquals(mass, self.vessel.mass)

    def test_part_resources(self):
        resources = next(iter(self.vessel.parts.with_title('BACC "Thumper" Solid Fuel Booster'))).resources
        self.assertEqual(set(['SolidFuel']), set(resources.names))
        self.assertTrue(resources.has_resource('SolidFuel'))
        self.assertFalse(resources.has_resource('LiquidFuel'))
        self.assertEqual(set(['SolidFuel']), set(resources.names))
        self.assertEqual(300, resources.amount('SolidFuel'))
        self.assertEqual(850, resources.max('SolidFuel'))

        resources = next(iter(self.vessel.parts.with_title('Rockomax X200-16 Fuel Tank'))).resources
        self.assertEqual(set(['LiquidFuel','Oxidizer']), set(resources.names))
        self.assertTrue(resources.has_resource('LiquidFuel'))
        self.assertTrue(resources.has_resource('Oxidizer'))
        self.assertFalse(resources.has_resource('SolidFuel'))
        self.assertEqual(300, resources.amount('LiquidFuel'))
        self.assertEqual(720, resources.max('LiquidFuel'))
        self.assertEqual(400, resources.amount('Oxidizer'))
        self.assertEqual(880, resources.max('Oxidizer'))

class TestResourcesStaticMethods(testingtools.TestCase, ResourcesTest):

    @classmethod
    def setUpClass(cls):
        cls.conn = testingtools.connect(name='TestResourcesStaticMethods')

    @classmethod
    def tearDownClass(cls):
        cls.conn.close()

    def test_density(self):
        Resources = self.conn.space_center.Resources
        for name,expected in self.density.items():
            self.assertEqual(expected, Resources.density(name))
        self.assertRaises(krpc.error.RPCError, Resources.density, 'Foo')

    def test_flow_mode(self):
        Resources = self.conn.space_center.Resources
        Mode = self.conn.space_center.ResourceFlowMode
        self.assertEqual(Mode.vessel, Resources.flow_mode('ElectricCharge'))
        self.assertEqual(Mode.vessel, Resources.flow_mode('IntakeAir'))
        self.assertEqual(Mode.stage, Resources.flow_mode('MonoPropellant'))
        self.assertEqual(Mode.stage, Resources.flow_mode('XenonGas'))
        self.assertEqual(Mode.adjacent, Resources.flow_mode('LiquidFuel'))
        self.assertEqual(Mode.adjacent, Resources.flow_mode('Oxidizer'))
        self.assertEqual(Mode.none, Resources.flow_mode('SolidFuel'))
        self.assertEqual(Mode.vessel, Resources.flow_mode('Ore'))
        self.assertEqual(Mode.none, Resources.flow_mode('Ablator'))
        self.assertRaises(krpc.error.RPCError, Resources.flow_mode, 'Foo')

if __name__ == "__main__":
    unittest.main()
